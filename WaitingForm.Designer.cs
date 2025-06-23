namespace FilesCompressionProject
{
    partial class WaitingForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnResume;
        private System.Windows.Forms.Label labelStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnResume = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(110, 100);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "إلغاء";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(20, 60);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(80, 30);
            this.btnPause.TabIndex = 1;
            this.btnPause.Text = "إيقاف مؤقت";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnResume
            // 
            this.btnResume.Location = new System.Drawing.Point(200, 60);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(80, 30);
            this.btnResume.TabIndex = 2;
            this.btnResume.Text = "استئناف";
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.Location = new System.Drawing.Point(40, 20);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(220, 20);
            this.labelStatus.TabIndex = 3;
            this.labelStatus.Text = "جاري المعالجة...";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WaitingForm
            // 
            this.ClientSize = new System.Drawing.Size(300, 150);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.btnResume);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnCancel);
            this.Name = "WaitingForm";
            this.Text = "انتظر قليلاً...";
            this.ResumeLayout(false);
        }
    }
}
