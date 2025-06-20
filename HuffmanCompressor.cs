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

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = openFileDialog.FileName;
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
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Please select the file first", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (isCancelled()) return;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;

            backgroundWorker.DoWork += (s, args) =>
            {
                var compressor = new HuffmanCompressor(() => cancelRequested);
                compressor.CompressFile(selectedFilePath);
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
                    MessageBox.Show(" تم إلغاء العملية", "تم الإلغاء",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                FileInfo original = new FileInfo(selectedFilePath);
                FileInfo compressed = new FileInfo("compressed.huff");

                long originalSize = original.Length;
                long compressedSize = compressed.Length;

                double ratio = (double)compressedSize / originalSize;
                double percentage = (1 - ratio) * 100;

                MessageBox.Show(
                    $"تم ضغط الملف بنجاح إلى compressed.huff\n\n" +
                    $"الحجم الأصلي: {originalSize} bytes\n" +
                    $"الحجم بعد الضغط: {compressedSize} bytes\n" +
                    $"نسبة الضغط: {ratio:F2}\n" +
                    $"نسبة التوفير: {percentage:F2}%",
                    "تمت العملية",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            };


            cancelRequested = false;
            waitingForm = new WaitingForm();
            Task.Run(() => backgroundWorker.RunWorkerAsync());
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
            compressor.DecompressFile("compressed.huff");
            MessageBox.Show("تم فك الضغط وحفظ الملف باسم decompressed.huff");
        }

        private bool isCancelled()
        {
            return cancelRequested;
        }
    }
}
