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

        private bool isCancelled()
        {
            return cancelRequested;
        }
    }
}
