namespace FilesCompressionProject
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.DecompressShannonFano = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CompressionHuffman
            // 
            this.CompressionHuffman.Location = new System.Drawing.Point(200, 128);
            this.CompressionHuffman.Name = "CompressionHuffman";
            this.CompressionHuffman.Size = new System.Drawing.Size(174, 23);
            this.CompressionHuffman.TabIndex = 0;
            this.CompressionHuffman.Text = "Compression Huffman";
            this.CompressionHuffman.UseVisualStyleBackColor = true;
            this.CompressionHuffman.Click += new System.EventHandler(this.CompressionHuffman_Click);
            // 
            // Decompress
            // 
            this.Decompress.Location = new System.Drawing.Point(449, 128);
            this.Decompress.Name = "Decompress";
            this.Decompress.Size = new System.Drawing.Size(169, 23);
            this.Decompress.TabIndex = 1;
            this.Decompress.Text = "Decompress Huffman";
            this.Decompress.UseVisualStyleBackColor = true;
            this.Decompress.Click += new System.EventHandler(this.Decompress_Click);
            // 
            // ChooseFile
            // 
            this.ChooseFile.Location = new System.Drawing.Point(30, 128);
            this.ChooseFile.Name = "ChooseFile";
            this.ChooseFile.Size = new System.Drawing.Size(121, 23);
            this.ChooseFile.TabIndex = 2;
            this.ChooseFile.Text = "Choose File";
            this.ChooseFile.UseVisualStyleBackColor = true;
            this.ChooseFile.Click += new System.EventHandler(this.ChooseFile_Click);
            // 
            // ChooseFolder
            // 
            this.ChooseFolder.Location = new System.Drawing.Point(30, 178);
            this.ChooseFolder.Name = "ChooseFolder";
            this.ChooseFolder.Size = new System.Drawing.Size(121, 23);
            this.ChooseFolder.TabIndex = 3;
            this.ChooseFolder.Text = "Choose Folder";
            this.ChooseFolder.UseVisualStyleBackColor = true;
            this.ChooseFolder.Click += new System.EventHandler(this.ChooseFolder_Click);
            // 
            // CompressionShannonFano
            // 
            this.CompressionShannonFano.Location = new System.Drawing.Point(200, 178);
            this.CompressionShannonFano.Name = "CompressionShannonFano";
            this.CompressionShannonFano.Size = new System.Drawing.Size(210, 23);
            this.CompressionShannonFano.TabIndex = 4;
            this.CompressionShannonFano.Text = "Compression Shannon Fano";
            this.CompressionShannonFano.UseVisualStyleBackColor = true;
            this.CompressionShannonFano.Click += new System.EventHandler(this.CompressionShannonFano_Click);
            // 
            // DecompressShannonFano
            // 
            this.DecompressShannonFano.Location = new System.Drawing.Point(430, 178);
            this.DecompressShannonFano.Name = "DecompressShannonFano";
            this.DecompressShannonFano.Size = new System.Drawing.Size(239, 23);
            this.DecompressShannonFano.TabIndex = 5;
            this.DecompressShannonFano.Text = "Decompress Shannon Fano";
            this.DecompressShannonFano.UseVisualStyleBackColor = true;
            this.DecompressShannonFano.Click += new System.EventHandler(this.DecompressShannonFano_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.DecompressShannonFano);
            this.Controls.Add(this.CompressionShannonFano);
            this.Controls.Add(this.ChooseFolder);
            this.Controls.Add(this.ChooseFile);
            this.Controls.Add(this.Decompress);
            this.Controls.Add(this.CompressionHuffman);
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
    }
}

