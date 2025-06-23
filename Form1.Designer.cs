﻿﻿﻿namespace FilesCompressionProject
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListBox archiveFilesListBox;
        private System.Windows.Forms.Button compressToArchiveButton;
        private System.Windows.Forms.Button browseArchiveButton;
        private System.Windows.Forms.Button extractSelectedFileButton;


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CompressionHuffman = new System.Windows.Forms.Button();
            this.Decompress = new System.Windows.Forms.Button();
            this.ChooseFile = new System.Windows.Forms.Button();
            this.ChooseFolder = new System.Windows.Forms.Button();
            this.CompressionShannonFano = new System.Windows.Forms.Button();
            this.archiveFilesListBox = new System.Windows.Forms.ListBox();
            this.compressToArchiveButton = new System.Windows.Forms.Button();
            this.browseArchiveButton = new System.Windows.Forms.Button();
            this.extractSelectedFileButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.DecompressShannonFano = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CompressionHuffman
            // 
            this.CompressionHuffman.Location = new System.Drawing.Point(150, 104);
            this.CompressionHuffman.Margin = new System.Windows.Forms.Padding(2);
            this.CompressionHuffman.Name = "CompressionHuffman";
            this.CompressionHuffman.Size = new System.Drawing.Size(130, 19);
            this.CompressionHuffman.TabIndex = 0;
            this.CompressionHuffman.Text = "Compression Huffman";
            this.CompressionHuffman.UseVisualStyleBackColor = true;
            this.CompressionHuffman.Click += new System.EventHandler(this.CompressionHuffman_Click);
            // 
            // Decompress
            // 
            this.Decompress.Location = new System.Drawing.Point(337, 104);
            this.Decompress.Margin = new System.Windows.Forms.Padding(2);
            this.Decompress.Name = "Decompress";
            this.Decompress.Size = new System.Drawing.Size(109, 19);
            this.Decompress.TabIndex = 1;
            this.Decompress.Text = "Decompress Huffman";
            this.Decompress.UseVisualStyleBackColor = true;
            this.Decompress.Click += new System.EventHandler(this.Decompress_Click);
            // 
            // ChooseFile
            // 
            this.ChooseFile.Location = new System.Drawing.Point(22, 104);
            this.ChooseFile.Margin = new System.Windows.Forms.Padding(2);
            this.ChooseFile.Name = "ChooseFile";
            this.ChooseFile.Size = new System.Drawing.Size(91, 19);
            this.ChooseFile.TabIndex = 2;
            this.ChooseFile.Text = "Choose File";
            this.ChooseFile.UseVisualStyleBackColor = true;
            this.ChooseFile.Click += new System.EventHandler(this.ChooseFile_Click);
            // 
            // ChooseFolder
            // 
            this.ChooseFolder.Location = new System.Drawing.Point(22, 145);
            this.ChooseFolder.Margin = new System.Windows.Forms.Padding(2);
            this.ChooseFolder.Name = "ChooseFolder";
            this.ChooseFolder.Size = new System.Drawing.Size(91, 19);
            this.ChooseFolder.TabIndex = 3;
            this.ChooseFolder.Text = "Choose Folder";
            this.ChooseFolder.UseVisualStyleBackColor = true;
            this.ChooseFolder.Click += new System.EventHandler(this.ChooseFolder_Click);
            // 
            // CompressionShannonFano
            // 
            this.CompressionShannonFano.Location = new System.Drawing.Point(150, 145);
            this.CompressionShannonFano.Margin = new System.Windows.Forms.Padding(2);
            this.CompressionShannonFano.Name = "CompressionShannonFano";
            this.CompressionShannonFano.Size = new System.Drawing.Size(144, 19);
            this.CompressionShannonFano.TabIndex = 4;
            this.CompressionShannonFano.Text = "Compression Shannon Fano";
            this.CompressionShannonFano.UseVisualStyleBackColor = true;
            this.CompressionShannonFano.Click += new System.EventHandler(this.CompressionShannonFano_Click);
            // 
            // archiveFilesListBox
            // 
            this.archiveFilesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.archiveFilesListBox.Location = new System.Drawing.Point(22, 200);
            this.archiveFilesListBox.Name = "archiveFilesListBox";
            this.archiveFilesListBox.Size = new System.Drawing.Size(200, 147);
            this.archiveFilesListBox.TabIndex = 6;
            // 
            // compressToArchiveButton
            // 
            this.compressToArchiveButton.Location = new System.Drawing.Point(300, 200);
            this.compressToArchiveButton.Name = "compressToArchiveButton";
            this.compressToArchiveButton.Size = new System.Drawing.Size(180, 25);
            this.compressToArchiveButton.TabIndex = 7;
            this.compressToArchiveButton.Text = "Compress To Archive";
            this.compressToArchiveButton.UseVisualStyleBackColor = true;
            this.compressToArchiveButton.Click += new System.EventHandler(this.CompressToArchiveButton_Click);
            // 
            // browseArchiveButton
            // 
            this.browseArchiveButton.Location = new System.Drawing.Point(300, 240);
            this.browseArchiveButton.Name = "browseArchiveButton";
            this.browseArchiveButton.Size = new System.Drawing.Size(180, 25);
            this.browseArchiveButton.TabIndex = 8;
            this.browseArchiveButton.Text = "Browse Archive";
            this.browseArchiveButton.UseVisualStyleBackColor = true;
            this.browseArchiveButton.Click += new System.EventHandler(this.BrowseArchiveButton_Click);
            // 
            // extractSelectedFileButton
            // 
            this.extractSelectedFileButton.Location = new System.Drawing.Point(300, 280);
            this.extractSelectedFileButton.Name = "extractSelectedFileButton";
            this.extractSelectedFileButton.Size = new System.Drawing.Size(180, 25);
            this.extractSelectedFileButton.TabIndex = 9;
            this.extractSelectedFileButton.Text = "Extract Selected File";
            this.extractSelectedFileButton.UseVisualStyleBackColor = true;
            this.extractSelectedFileButton.Click += new System.EventHandler(this.ExtractSelectedFileButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(337, 143);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "compress folder";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // DecompressShannonFano
            // 
            this.DecompressShannonFano.Location = new System.Drawing.Point(300, 169);
            this.DecompressShannonFano.Name = "DecompressShannonFano";
            this.DecompressShannonFano.Size = new System.Drawing.Size(180, 23);
            this.DecompressShannonFano.TabIndex = 5;
            this.DecompressShannonFano.Text = "Decompress Shannon Fano";
            this.DecompressShannonFano.UseVisualStyleBackColor = true;
            this.DecompressShannonFano.Click += new System.EventHandler(this.DecompressShannonFano_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 366);
            this.Controls.Add(this.DecompressShannonFano);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.CompressionShannonFano);
            this.Controls.Add(this.ChooseFolder);
            this.Controls.Add(this.ChooseFile);
            this.Controls.Add(this.Decompress);
            this.Controls.Add(this.CompressionHuffman);
            this.Controls.Add(this.archiveFilesListBox);
            this.Controls.Add(this.compressToArchiveButton);
            this.Controls.Add(this.browseArchiveButton);
            this.Controls.Add(this.extractSelectedFileButton);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CompressionHuffman;
        private System.Windows.Forms.Button Decompress;
        private System.Windows.Forms.Button ChooseFile;
        private System.Windows.Forms.Button ChooseFolder;
        private System.Windows.Forms.Button CompressionShannonFano;
        private System.Windows.Forms.Button DecompressShannonFano;
        private System.Windows.Forms.Button button1;
    }
}
