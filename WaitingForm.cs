using System;
using System.Windows.Forms;

namespace FilesCompressionProject
{
	public partial class WaitingForm : Form
	{
		public bool IsCancelled { get; private set; }

		public WaitingForm()
		{
			InitializeComponent();
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			IsCancelled = true;
			Close();
		}
	}
}
