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
        private int selectedFolderPath = 0;

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
            selectedFolderPath++;

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
            if (string.IsNullOrEmpty(selectedFilePath)|| selectedFolderPath == 0)
            {
                MessageBox.Show("Please select the file or folder first", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                var compressor = new HuffmanCompressor();
                compressor.CompressFile(selectedFilePath);


                MessageBox.Show("تم ضغط الملف بنجاح compressed.huff");
            }
         
          


        }

        private void Decompress_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Please select the file or folder first ", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
          
            var compressor = new HuffmanCompressor();
            compressor.DecompressFile("compressed.huff");
            MessageBox.Show("تم فك الضغط وحفظ الملف باسم decompressed.huff");


        }


        private void CompressionShannonFano_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath)|| selectedFolderPath == 0)
            {
                MessageBox.Show("Please select the file  or Folder first ", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                var compressor = new ShannonFanoCompressor();
                compressor.CompressFile(selectedFilePath);
                MessageBox.Show("تم ضغط الملف بنجاح compressed_sf.shf");
            }
           

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
            var compressor = new ShannonFanoCompressor();
            compressor.DecompressFile(selectedFilePath);

        }

    

        
    }
}
//mustafa