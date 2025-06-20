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

namespace FilesCompressionProject
{
    public partial class Form1 : Form
    {

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

            var compressor = new HuffmanCompressor();
            foreach (string filePath in selectedFilePaths)
            {
                string directory = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string outputPath = Path.Combine(directory, fileName + ".huff");

                compressor.CompressFile(filePath, outputPath);

                FileInfo original = new FileInfo(filePath);
                FileInfo compressed = new FileInfo(outputPath);

                long originalSize = original.Length;
                long compressedSize = compressed.Length;

                double ratio = (double)compressedSize / originalSize;
                double percentage = (1 - ratio) * 100;

                MessageBox.Show(
                    $"تم ضغط الملف {fileName} بنجاح إلى {outputPath}\n\n" +
                    $"الحجم الأصلي: {originalSize} bytes\n" +
                    $"الحجم بعد الضغط: {compressedSize} bytes\n" +
                    $"نسبة التوفير: {percentage:F2}%",
                    "تمت العملية",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
        private void CompressionShannonFano_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Please select the file first", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

        }

        private void Decompress_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Please select the file first", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var compressor = new HuffmanCompressor();
            string directory = Path.GetDirectoryName(selectedFilePath);
            string compressedPath = selectedFilePath;

            string originalName = Path.GetFileNameWithoutExtension(selectedFilePath);
            string decompressedPath = Path.Combine(directory, originalName );

            compressor.DecompressFile(compressedPath, decompressedPath);

            MessageBox.Show("decompressed.huffتم فك الضغط وحفظ الملف باسم ");


        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
