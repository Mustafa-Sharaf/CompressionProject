using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FilesCompressionProject;

namespace FilesCompressionProject
{
    public partial class Form1 : Form
    {
        private BackgroundWorker backgroundWorker;
        private WaitingForm waitingForm;
        private BackgroundWorker compressWorker;
        private BackgroundWorker decompressWorker;
        private bool cancelRequested = false;
        private BackgroundWorker extractWorker;


        private string selectedFilePath = string.Empty;
        private string selectedFolderPath = string.Empty;

        private List<string> selectedFilePaths = new List<string>();
        private List<HuffmanArchiveEntry> archiveEntries = new List<HuffmanArchiveEntry>();
        public Form1()
        {
            InitializeComponent();
        }

        private void ChooseFile_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select a file";
                openFileDialog.Filter = "All Files (*.*)|*.*";
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePaths = openFileDialog.FileNames.ToList();

                    if (selectedFilePaths.Count == 1)
                        selectedFilePath = selectedFilePaths[0];
                }
            }
        }

        private void ChooseFolder_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFolderPath = folderDialog.SelectedPath;

                }
            }
        }

        private void CompressionHuffman_Click(object sender, EventArgs e)
        {
            if (selectedFilePaths == null || selectedFilePaths.Count == 0)
            {
                MessageBox.Show("Please select one or more files first.", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            cancelRequested = false;

            waitingForm = new WaitingForm();
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;

            List<(string filePath, string outputPath)> results = new List<(string filePath, string outputPath)>();

            backgroundWorker.DoWork += (s, args) =>
            {
                var compressor = new HuffmanCompressor(() => cancelRequested);

                foreach (string filePath in selectedFilePaths)
                {
                    if (cancelRequested) break;

                    string directory = Path.GetDirectoryName(filePath);
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string outputPath = Path.Combine(directory, fileName + ".huff");

                    compressor.CompressFile(filePath, outputPath);
                    if (File.Exists(outputPath))
                    {
                        results.Add((filePath, outputPath));
                    }
                }
            };

            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                if (waitingForm.InvokeRequired)
                {
                    waitingForm.Invoke((MethodInvoker)(() => waitingForm.Close()));
                }
                else
                {
                    waitingForm.Close();
                }

                if (cancelRequested)
                {
                    MessageBox.Show("تم إلغاء العملية",
                     "تم الإلغاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (results.Count == 0)
                {
                    return;
                }

                StringBuilder resultMessage = new StringBuilder();


                foreach (var (filePath, outputPath) in results)
                {
                    FileInfo original = new FileInfo(filePath);
                    FileInfo compressed = new FileInfo(outputPath);

                    long originalSize = original.Length;
                    long compressedSize = compressed.Length;

                    double ratio = (double)compressedSize / originalSize;
                    double percentage = (1 - ratio) * 100;

                    resultMessage.AppendLine($"الملف: {Path.GetFileName(filePath)}");
                    resultMessage.AppendLine($"الحجم الأصلي: {originalSize} bytes");
                    resultMessage.AppendLine($"الحجم بعد الضغط: {compressedSize} bytes");
                    resultMessage.AppendLine($"نسبة الضغط: {ratio:F2}");
                    resultMessage.AppendLine($"نسبة التوفير: {percentage:F2}%");
                    resultMessage.AppendLine("---------------------------------------");
                }

                MessageBox.Show(resultMessage.ToString(), "نتائج الضغط", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            backgroundWorker.RunWorkerAsync();
            waitingForm.ShowDialog();
            cancelRequested = waitingForm.IsCancelled;
        }

        private void Decompress_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Please select the file first", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (isCancelled()) return;

            var compressor = new HuffmanCompressor();
            string directory = Path.GetDirectoryName(selectedFilePath);
            string compressedPath = selectedFilePath;

            string originalName = Path.GetFileNameWithoutExtension(selectedFilePath);
            string decompressedPath = Path.Combine(directory, originalName);

            bool success = compressor.DecompressFile(compressedPath, decompressedPath);

            if (success)
            {
                MessageBox.Show("تم فك الضغط وحفظ الملف بنجاح.");
            }
            else
            {
            }



        }

        
        private void CompressToArchiveButton_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Multiselect = true;
                openDialog.Title = "Select files to compress";
                if (openDialog.ShowDialog() != DialogResult.OK) return;

                cancelRequested = false;
                waitingForm = new WaitingForm();
                compressWorker = new BackgroundWorker();
                compressWorker.WorkerSupportsCancellation = true;

                string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                string archivePath = Path.Combine(downloadsPath, DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_compressed.huffarc");
                List<HuffmanArchiveEntry> entries = new List<HuffmanArchiveEntry>();

                compressWorker.DoWork += (s, args) =>
                {
                    using (FileStream archive = new FileStream(archivePath, FileMode.Create))
                    {
                        archive.Seek(1024, SeekOrigin.Begin); // احجز 1KB كبداية للجدول

                        foreach (string file in openDialog.FileNames)
                        {
                            if (cancelRequested)
                            {
                                args.Cancel = true;
                                break;
                            }

                            byte[] inputBytes = File.ReadAllBytes(file);
                            byte[] compressedBytes = new HuffmanCompressor().CompressBytes(inputBytes);

                            if (compressedBytes == null || compressedBytes.Length == 0)
                            {
                                MessageBox.Show($"لم يتم ضغط الملف {Path.GetFileName(file)} بسبب عدم إدخال كلمة مرور أو وجود خطأ.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                continue;
                            }

                            long offset = archive.Position;
                            archive.Write(compressedBytes, 0, compressedBytes.Length);

                            entries.Add(new HuffmanArchiveEntry
                            {
                                FileName = Path.GetFileName(file),
                                OriginalSize = inputBytes.Length,
                                CompressedSize = compressedBytes.Length,
                                Offset = offset
                            });
                        }

                        if (!args.Cancel)
                        {
                            // كتابة جدول المحتويات في البداية
                            archive.Seek(0, SeekOrigin.Begin);
                            using (BinaryWriter writer = new BinaryWriter(archive, Encoding.UTF8, true))
                            {
                                writer.Write(entries.Count);
                                foreach (var entry in entries)
                                {
                                    writer.Write(entry.FileName);
                                    writer.Write(entry.OriginalSize);
                                    writer.Write(entry.CompressedSize);
                                    writer.Write(entry.Offset);
                                }
                            }
                        }
                    }
                };

                compressWorker.RunWorkerCompleted += (s, args) =>
                {
                    if (waitingForm.InvokeRequired)
                    {
                        waitingForm.Invoke((MethodInvoker)(() => waitingForm.Close()));
                    }
                    else
                    {
                        waitingForm.Close();
                    }

                    if (args.Cancelled)
                    {
                        MessageBox.Show("تم إلغاء الضغط", "تم الإلغاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    MessageBox.Show("تم ضغط الملفات في مجلد التنزيلات داخل compressed.huffarc");
                };

                compressWorker.RunWorkerAsync();
                waitingForm.ShowDialog();
                cancelRequested = waitingForm.IsCancelled;
            }
        }

        private void BrowseArchiveButton_Click(object sender, EventArgs e)
        {
            archiveFilesListBox.Items.Clear();
            archiveEntries = new List<HuffmanArchiveEntry>();

            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = downloadsPath;
                openFileDialog.Filter = "Huffman Archives (*.huffarc)|*.huffarc";
                openFileDialog.Title = "اختر ملف أرشيف (.huffarc)";

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string archivePath = openFileDialog.FileName;

                if (!File.Exists(archivePath))
                {
                    MessageBox.Show("ملف الأرشيف غير موجود.");
                    return;
                }

                using (var fs = new FileStream(archivePath, FileMode.Open))
                using (var reader = new BinaryReader(fs, Encoding.UTF8))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        var entry = new HuffmanArchiveEntry
                        {
                            FileName = reader.ReadString(),
                            OriginalSize = reader.ReadInt64(),
                            CompressedSize = reader.ReadInt64(),
                            Offset = reader.ReadInt64()
                        };
                        archiveEntries.Add(entry);
                        archiveFilesListBox.Items.Add(entry.FileName);
                    }
                }
            }
        }

        private void ExtractSelectedFileButton_Click(object sender, EventArgs e)
        {
            if (archiveFilesListBox.SelectedIndex == -1)
            {
                MessageBox.Show("اختر ملفًا من القائمة أولاً.");
                return;
            }

            cancelRequested = false;
            waitingForm = new WaitingForm();
            extractWorker = new BackgroundWorker();
            extractWorker.WorkerSupportsCancellation = true;

            string selectedFileName = archiveFilesListBox.SelectedItem.ToString();
            var entry = archiveEntries.FirstOrDefault(ee => ee.FileName == selectedFileName);

            if (entry == null) return;

            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string archivePath = Path.Combine(downloadsPath, "compressed.huffarc");

            extractWorker.DoWork += (s, args) =>
            {
                if (cancelRequested)
                {
                    args.Cancel = true;
                    return;
                }

                using (var fs = new FileStream(archivePath, FileMode.Open))
                {
                    fs.Seek(entry.Offset, SeekOrigin.Begin);
                    byte[] compressedData = new byte[entry.CompressedSize];
                    fs.Read(compressedData, 0, compressedData.Length);

                    if (cancelRequested)
                    {
                        args.Cancel = true;
                        return;
                    }

                    byte[] decompressedData = new HuffmanCompressor().DecompressBytes(compressedData);

                    if (cancelRequested)
                    {
                        args.Cancel = true;
                        return;
                    }

                    string savePath = Path.Combine(downloadsPath, "extracted_" + entry.FileName);
                    File.WriteAllBytes(savePath, decompressedData);
                }
            };

            extractWorker.RunWorkerCompleted += (s, args) =>
            {
                if (waitingForm.InvokeRequired)
                {
                    waitingForm.Invoke((MethodInvoker)(() => waitingForm.Close()));
                }
                else
                {
                    waitingForm.Close();
                }

                if (args.Cancelled)
                {
                    MessageBox.Show("تم إلغاء فك الضغط", "تم الإلغاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                MessageBox.Show("تم استخراج الملف إلى: مجلد التنزيلات\n\nextracted_" + entry.FileName);
            };

            extractWorker.RunWorkerAsync();
            waitingForm.ShowDialog();
            cancelRequested = waitingForm.IsCancelled;
        }

        private bool isCancelled()
        {
            return cancelRequested;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFolderPath) || !Directory.Exists(selectedFolderPath))
            {
                MessageBox.Show("الرجاء اختيار مجلد ", "مجلد غير محدد", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            cancelRequested = false;
            waitingForm = new WaitingForm();
            compressWorker = new BackgroundWorker();
            compressWorker.WorkerSupportsCancellation = true;

            string archivePath = "";

            compressWorker.DoWork += (s, args) =>
            {
                var compressor = new HuffmanCompressor(() => cancelRequested);
                archivePath = compressor.CompressFolder(selectedFolderPath);
            };

            compressWorker.RunWorkerCompleted += (s, args) =>
            {
                if (waitingForm.InvokeRequired)
                    waitingForm.Invoke((MethodInvoker)(() => waitingForm.Close()));
                else
                    waitingForm.Close();

                if (cancelRequested || args.Cancelled)
                {
                    MessageBox.Show("تم إلغاء ضغط المجلد", "إلغاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                MessageBox.Show("تم ضغط المجلد إلى:\n" + archivePath, "نجاح");
            };

            compressWorker.RunWorkerAsync();
            waitingForm.ShowDialog();
            cancelRequested = waitingForm.IsCancelled;
        }


        private void CompressionShannonFano_Click(object sender, EventArgs e)
        {
            if (selectedFilePaths == null || selectedFilePaths.Count == 0 || selectedFilePaths.Any(f => !File.Exists(f)))
            {
                MessageBox.Show("يرجى اختيار ملف صالح واحد على الأقل", "ملف مفقود", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            cancelRequested = false;
            waitingForm = new WaitingForm();
            compressWorker = new BackgroundWorker();
            compressWorker.WorkerSupportsCancellation = true;

            List<string> compressedPaths = new List<string>();

            compressWorker.DoWork += (s, args) =>
            {
                try
                {
                    foreach (var inputPath in selectedFilePaths)
                    {
                        if (cancelRequested)
                            throw new OperationCanceledException();

                        var compressor = new ShannonFanoCompressor(() => cancelRequested);
                        string compressedPath = compressor.CompressFile(inputPath);
                        compressedPaths.Add(compressedPath);
                    }
                }
                catch (OperationCanceledException)
                {
                    args.Cancel = true;
                }
            };

            compressWorker.RunWorkerCompleted += (s, args) =>
            {
                if (waitingForm.InvokeRequired)
                    waitingForm.Invoke((MethodInvoker)(() => waitingForm.Close()));
                else
                    waitingForm.Close();

                if (cancelRequested || args.Cancelled)
                {
                    MessageBox.Show("تم إلغاء عملية الضغط", "إلغاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                StringBuilder message = new StringBuilder();
                message.AppendLine("تم ضغط الملفات بنجاح:\n");

                foreach (var compressedPath in compressedPaths)
                {
                    var compressedFile = new FileInfo(compressedPath);

                    message.AppendLine($"{Path.GetFileName(compressedPath)}");
                    message.AppendLine($"الحجم بعد الضغط: {compressedFile.Length} bytes");
                    message.AppendLine();
                }

                MessageBox.Show(message.ToString(), "تمت العملية", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            compressWorker.RunWorkerAsync();
            waitingForm.ShowDialog();
            cancelRequested = waitingForm.IsCancelled;

        }


        private void DecompressShannonFano_Click(object sender, EventArgs e)
        {


            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "اختر ملف مضغوط بصيغة Shannon-Fano";
                openFileDialog.Filter = "Shannon-Fano Compressed Files (*.shf)|*.shf";

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                selectedFilePath = openFileDialog.FileName;
            }

            cancelRequested = false;
            waitingForm = new WaitingForm();
            decompressWorker = new BackgroundWorker();
            decompressWorker.WorkerSupportsCancellation = true;

            decompressWorker.DoWork += (s, args) =>
            {
                try
                {
                    var compressor = new ShannonFanoCompressor(() => cancelRequested);
                    compressor.DecompressFile(selectedFilePath, () => cancelRequested);
                }
                catch (OperationCanceledException)
                {
                    args.Cancel = true;
                }
            };

            decompressWorker.RunWorkerCompleted += (s, args) =>
            {
                if (waitingForm.InvokeRequired)
                    waitingForm.Invoke((MethodInvoker)(() => waitingForm.Close()));
                else
                    waitingForm.Close();

                if (cancelRequested || args.Cancelled)
                {
                    MessageBox.Show("تم إلغاء عملية فك الضغط", "إلغاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            };

            decompressWorker.RunWorkerAsync();
            waitingForm.ShowDialog();
            cancelRequested = waitingForm.IsCancelled;

        }

       
    }
}
//mustafa