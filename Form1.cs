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
        private bool cancelRequested = false;

        private string selectedFilePath = string.Empty;
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
                    string folderPath = folderDialog.SelectedPath;
                    string[] files = Directory.GetFiles(folderPath);
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
                    results.Add((filePath, outputPath));
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
                    MessageBox.Show("تم إلغاء العملية", "تم الإلغاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void CompressionShannonFano_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Please select the file first", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (isCancelled()) return;
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
            string decompressedPath = Path.Combine(directory, originalName );

            compressor.DecompressFile(compressedPath, decompressedPath);

            MessageBox.Show("decompressed.huffتم فك الضغط وحفظ الملف باسم ");


        }

        private void CompressToArchiveButton_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Multiselect = true;
                openDialog.Title = "Select files to compress";
                if (openDialog.ShowDialog() != DialogResult.OK) return;

                string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                string archivePath = Path.Combine(downloadsPath, "compressed.huffarc");
                List<HuffmanArchiveEntry> entries = new List<HuffmanArchiveEntry>();

                using (FileStream archive = new FileStream(archivePath, FileMode.Create))
                {
                    archive.Seek(1024, SeekOrigin.Begin); // احجز 1KB كبداية للجدول

                    foreach (string file in openDialog.FileNames)
                    {
                        byte[] inputBytes = File.ReadAllBytes(file);
                        byte[] compressedBytes = new HuffmanCompressor().CompressBytes(inputBytes);

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

                MessageBox.Show("تم ضغط الملفات في مجلد التنزيلات داخل compressed.huffarc");
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

            string selectedFileName = archiveFilesListBox.SelectedItem.ToString();
            var entry = archiveEntries.FirstOrDefault(ee => ee.FileName == selectedFileName);

            if (entry == null) return;

            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string archivePath = Path.Combine(downloadsPath, "compressed.huffarc");

            using (var fs = new FileStream(archivePath, FileMode.Open))
            {
                fs.Seek(entry.Offset, SeekOrigin.Begin);
                byte[] compressedData = new byte[entry.CompressedSize];
                fs.Read(compressedData, 0, compressedData.Length);

                byte[] decompressedData = new HuffmanCompressor().DecompressBytes(compressedData);
                string savePath = Path.Combine(downloadsPath, "extracted_" + entry.FileName);
                File.WriteAllBytes(savePath, decompressedData);
            }

            MessageBox.Show("تم استخراج الملف إلى: مجلد التنزيلات\n\nextracted_" + entry.FileName);
        }


        private bool isCancelled()
        {
            return cancelRequested;
        }
    }
}
