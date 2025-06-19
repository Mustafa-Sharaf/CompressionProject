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


        private int selectingAlgorithm = 0;
        public Form1()
        {
            InitializeComponent();
        }

       
        private void ChooseFile_Click(object sender, EventArgs e)
        {
            if (selectingAlgorithm == 0)
            {
                MessageBox.Show("Please select the algorithm before", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select a file";
                openFileDialog.Filter = "All Files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    //MessageBox.Show($" تم اختيار الملف:\n{filePath}");
                }
            }
        }

        private void ChooseFolder_Click(object sender, EventArgs e)
        {
            if (selectingAlgorithm == 0)
            {
                MessageBox.Show("Please select the algorithm before", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderDialog.SelectedPath;
                    string[] files = Directory.GetFiles(folderPath);

                    //MessageBox.Show($"📁 تم اختيار المجلد:\n{folderPath}\nيحتوي على {files.Length} ملف");
                }
            }

        }


        private void CompressionHuffman_Click(object sender, EventArgs e)
        {
            selectingAlgorithm++;

        }
         private void CompressionShannonFano_Click(object sender, EventArgs e)
        {
            selectingAlgorithm++;

        }

        private void Decompress_Click(object sender, EventArgs e)
        {

        }

       
    }
}
