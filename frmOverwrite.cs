using System;
using System.IO;
using System.Windows.Forms;

namespace JDP {
	internal partial class frmOverwrite : Form {
		private bool _toAll;

		public frmOverwrite(string path) {
			InitializeComponent();
			Program.SetFontAndScaling(this);

			lblFileName.Text = Path.GetFileName(path);
		}

		public bool ToAll {
			get {
				return _toAll;
			}
		}

		private void btnYes_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Yes;
		}

		private void btnYesAll_Click(object sender, EventArgs e) {
			_toAll = true;
			DialogResult = DialogResult.Yes;
		}

		private void btnNo_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.No;
		}

		private void btnNoAll_Click(object sender, EventArgs e) {
			_toAll = true;
			DialogResult = DialogResult.No;
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}
	}
}
