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
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CompressionHuffman
            // 
            this.CompressionHuffman.Location = new System.Drawing.Point(150, 104);
            this.CompressionHuffman.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.Decompress.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Decompress.Name = "Decompress";
            this.Decompress.Size = new System.Drawing.Size(56, 19);
            this.Decompress.TabIndex = 1;
            this.Decompress.Text = "Decompress";
            this.Decompress.UseVisualStyleBackColor = true;
            this.Decompress.Click += new System.EventHandler(this.Decompress_Click);
            // 
            // ChooseFile
            // 
            this.ChooseFile.Location = new System.Drawing.Point(22, 104);
            this.ChooseFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.ChooseFolder.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.CompressionShannonFano.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.CompressionShannonFano.Name = "CompressionShannonFano";
            this.CompressionShannonFano.Size = new System.Drawing.Size(144, 19);
            this.CompressionShannonFano.TabIndex = 4;
            this.CompressionShannonFano.Text = "Compression Shannon Fano";
            this.CompressionShannonFano.UseVisualStyleBackColor = true;
            this.CompressionShannonFano.Click += new System.EventHandler(this.CompressionShannonFano_Click);
            // 
            //// button1
            //// 
            //this.button1.Location = new System.Drawing.Point(337, 141);
            //this.button1.Name = "button1";
            //this.button1.Size = new System.Drawing.Size(56, 23);
            //this.button1.TabIndex = 5;
            //this.button1.Text = "Cancel";
            //this.button1.UseVisualStyleBackColor = true;
            //this.button1.Click += new System.EventHandler(this.button1_Click);
            //// 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 366);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.CompressionShannonFano);
            this.Controls.Add(this.ChooseFolder);
            this.Controls.Add(this.ChooseFile);
            this.Controls.Add(this.Decompress);
            this.Controls.Add(this.CompressionHuffman);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
        private System.Windows.Forms.Button button1;
    }
}

