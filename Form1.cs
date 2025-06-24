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
using System.Threading;

namespace FilesCompressionProject
{
    public partial class Form1 : Form
    {
        private ManualResetEvent pauseEvent = new ManualResetEvent(true);
        private BackgroundWorker backgroundWorker;
        private WaitingForm waitingForm;
        private BackgroundWorker compressWorker;
        private BackgroundWorker decompressWorker;
        private bool cancelRequested = false;
        private BackgroundWorker extractWorker;
        private string lastArchivePath = "";




        private string selectedFilePath = string.Empty;
        private string selectedFolderPath = string.Empty;

        private List<string> selectedFilePaths = new List<string>();
        private List<HuffmanArchiveEntry> archiveEntries = new List<HuffmanArchiveEntry>();
        public Form1()
        {
            InitializeComponent();
        }
        private void PauseButton_Click(object sender, EventArgs e)
        {
            pauseEvent.Reset(); // يوقف مؤقتًا
           
        }
        private void ResumeButton_Click(object sender, EventArgs e)
        {
            pauseEvent.Set(); // يستأنف
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
            waitingForm.PauseRequested += () => pauseEvent.Reset();
            waitingForm.ResumeRequested += () => pauseEvent.Set();

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
                pauseEvent.WaitOne();
                openDialog.Multiselect = true;
                openDialog.Title = "Select files to compress";
                if (openDialog.ShowDialog() != DialogResult.OK) return;
                pauseEvent.WaitOne();
                cancelRequested = false;
                waitingForm = new WaitingForm();
                waitingForm.PauseRequested += () => pauseEvent.Reset();
                waitingForm.ResumeRequested += () => pauseEvent.Set();
                pauseEvent.WaitOne();
                compressWorker = new BackgroundWorker();
                compressWorker.WorkerSupportsCancellation = true;
                pauseEvent.WaitOne();
                string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                string archivePath = Path.Combine(downloadsPath, DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_compressed.huffarc");
                lastArchivePath = archivePath;
                pauseEvent.WaitOne();
                List<HuffmanArchiveEntry> entries = new List<HuffmanArchiveEntry>();
                object archiveLock = new object();
                pauseEvent.WaitOne();
                compressWorker.DoWork += (s, args) =>
                {
                    pauseEvent.WaitOne();

                    string password = HuffmanCompressor.PromptForPassword();
                     if (string.IsNullOrWhiteSpace(password))//5
                    {
                        MessageBox.Show("لم يتم إدخال كلمة المرور. تم إلغاء الضغط.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        args.Cancel = true; return;
                    }
                    using (FileStream archive = new FileStream(archivePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        pauseEvent.WaitOne();
                        archive.Seek(1024, SeekOrigin.Begin);
                        pauseEvent.WaitOne();
                        Parallel.ForEach(openDialog.FileNames, (file, state) =>
                        {
                            pauseEvent.WaitOne();

                            if (cancelRequested)
                            {
                                args.Cancel = true;
                                state.Stop();
                                return;
                            }

                            byte[] inputBytes = File.ReadAllBytes(file);
                            byte[] compressedBytes = new HuffmanCompressor().CompressBytes(inputBytes,password);

                            long offset;

                            lock (archiveLock)
                            {
                                pauseEvent.WaitOne();
                                offset = archive.Position;
                                archive.Write(compressedBytes, 0, compressedBytes.Length);
                            }
                            pauseEvent.WaitOne();
                            lock (entries)
                            {
                                pauseEvent.WaitOne();
                                entries.Add(new HuffmanArchiveEntry
                                {
                                  
                                    FileName = Path.GetFileName(file),
                                    OriginalSize = inputBytes.Length,
                                    CompressedSize = compressedBytes.Length,
                                    Offset = offset
                                });
                            }
                        });
                        pauseEvent.WaitOne();
                        if (!args.Cancel)
                        {
                            pauseEvent.WaitOne();
                            lock (archiveLock)
                            {
                                pauseEvent.WaitOne();
                                archive.Seek(0, SeekOrigin.Begin);
                                using (BinaryWriter writer = new BinaryWriter(archive, Encoding.UTF8, true))
                                {
                                    pauseEvent.WaitOne();
                                    writer.Write(entries.Count);
                                    foreach (var entry in entries)
                                    {
                                        pauseEvent.WaitOne();
                                        writer.Write(entry.FileName);
                                        writer.Write(entry.OriginalSize);
                                        writer.Write(entry.CompressedSize);
                                        writer.Write(entry.Offset);
                                    }
                                }
                            }
                        }
                    }
                };
                pauseEvent.WaitOne();
                compressWorker.RunWorkerCompleted += (s, args) =>
                {
                    pauseEvent.WaitOne();
                    if (waitingForm.InvokeRequired)
                    {
                        pauseEvent.WaitOne();
                        waitingForm.Invoke((MethodInvoker)(() => waitingForm.Close()));
                    }
                    else
                    {
                        pauseEvent.WaitOne();
                        waitingForm.Close();
                    }

                    if (args.Cancelled || cancelRequested)
                    {
                        pauseEvent.WaitOne();
                        MessageBox.Show("تم إلغاء الضغط", "تم الإلغاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    pauseEvent.WaitOne();
                    MessageBox.Show("تم ضغط الملفات في مجلد التنزيلات داخل compressed.huffarc");
                };
                pauseEvent.WaitOne();
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

                lastArchivePath = archivePath; // ✅ هذا هو السطر الضروري لإصلاح الخطأ

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
            pauseEvent.WaitOne();
            cancelRequested = false;
            waitingForm = new WaitingForm();
            waitingForm.PauseRequested += () => pauseEvent.Reset();
            waitingForm.ResumeRequested += () => pauseEvent.Set();
            pauseEvent.WaitOne();
            extractWorker = new BackgroundWorker();
            extractWorker.WorkerSupportsCancellation = true;
            pauseEvent.WaitOne();
            string selectedFileName = archiveFilesListBox.SelectedItem.ToString();
            var entry = archiveEntries.FirstOrDefault(ee => ee.FileName == selectedFileName);
            pauseEvent.WaitOne();
            if (entry == null) return;
            pauseEvent.WaitOne();
            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string archivePath = lastArchivePath;
            if (string.IsNullOrEmpty(archivePath) || !File.Exists(archivePath))
            {
                pauseEvent.WaitOne();
                MessageBox.Show("لم يتم العثور على ملف الأرشيف.\nيرجى ضغط ملفات أولاً أو تحديد ملف الأرشيف.", "ملف غير موجود", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            pauseEvent.WaitOne();
            extractWorker.DoWork += (s, args) =>
            {
                pauseEvent.WaitOne();
                if (cancelRequested) { args.Cancel = true; return; }

                pauseEvent.WaitOne();
                using (var fs = new FileStream(archivePath, FileMode.Open))
                {
                    pauseEvent.WaitOne();
                    fs.Seek(entry.Offset, SeekOrigin.Begin);
                    byte[] compressedData = new byte[entry.CompressedSize];
                    fs.Read(compressedData, 0, compressedData.Length);
                    pauseEvent.WaitOne();
                    if (cancelRequested) { args.Cancel = true; return; }
                    pauseEvent.WaitOne();
                    byte[] decompressedData = new HuffmanCompressor().DecompressBytes(compressedData);
                    pauseEvent.WaitOne();
                    if (cancelRequested) { args.Cancel = true; return; }
                    pauseEvent.WaitOne();
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
            waitingForm.PauseRequested += () => pauseEvent.Reset();
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
                waitingForm.PauseRequested += () => pauseEvent.Reset();
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
            waitingForm.PauseRequested += () => pauseEvent.Reset();
            waitingForm.ResumeRequested += () => pauseEvent.Set();

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
//