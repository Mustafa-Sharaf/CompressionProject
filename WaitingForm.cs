using System;
using System.Windows.Forms;

namespace FilesCompressionProject
{
    public partial class WaitingForm : Form
    {
        public bool IsCancelled { get; private set; }
        public bool IsPaused { get; private set; }

        public event Action PauseRequested;
        public event Action ResumeRequested;

        public WaitingForm()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            IsCancelled = true;
            Close();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            IsPaused = true;
            PauseRequested?.Invoke(); // إشعار الفورم الرئيسي
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            IsPaused = false;
            ResumeRequested?.Invoke(); // إشعار الفورم الرئيسي
        }
    }
}
