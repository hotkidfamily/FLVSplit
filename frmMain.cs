// --------------------------------------------------------------------------------
// Copyright (c) 2006 J.D. Purcell
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// --------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Windows.Forms;

namespace JDP
{
    internal partial class frmMain : Form {
		private Thread _statusThread;

		public frmMain() {
			InitializeComponent();
			Program.SetFontAndScaling(this);
			Text = $"FlvExtract {VersionInfo.DisplayVersion}";
		}


        private void LoadSettings() {
			SettingsReader sr = new SettingsReader("FLV Extract", "settings.txt");
			string val;

			if ((val = sr.Load("ExtractVideo")) != null) {
				chkVideo.Checked = (val != "0");
			}
			if ((val = sr.Load("ExtractTimeCodes")) != null) {
				chkTimeCodes.Checked = (val != "0");
			}
			if ((val = sr.Load("ExtractAudio")) != null) {
				chkAudio.Checked = (val != "0");
			}
            if ((val = sr.Load("Transcode")) != null)
            {
                chkTranscode.Checked = (val != "0");
            }
        }

		private void SaveSettings() {
			SettingsWriter sw = new SettingsWriter("FLV Extract", "settings.txt");

			sw.Save("ExtractVideo", chkVideo.Checked ? "1" : "0");
			sw.Save("ExtractTimeCodes", chkTimeCodes.Checked ? "1" : "0");
			sw.Save("ExtractAudio", chkAudio.Checked ? "1" : "0");
            sw.Save("Transcode", chkTranscode.Checked ? "1" : "0");

            sw.Close();
		}

		private void btnAbout_Click(object sender, EventArgs e) {
			string text = String.Format(" FLV Extract v{1}{0} Copyright :{2}{0} Authors :{3} {0} {4}",
				Environment.NewLine,
				VersionInfo.DisplayVersion,
				VersionInfo.CopyrightYears,
				VersionInfo.Authors,
				VersionInfo.Website);
			MessageBox.Show(this, text, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void frmMain_DragEnter(object sender, DragEventArgs e) {
			if ((_statusThread != null) && _statusThread.IsAlive) return;

			if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
				e.Effect = DragDropEffects.Copy;
			}
		}

		private void frmMain_DragDrop(object sender, DragEventArgs e) {
			if ((_statusThread != null) && _statusThread.IsAlive) return;

			if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
				string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
				_statusThread = new Thread(delegate() {
					Invoke((MethodInvoker)delegate() {
						using (frmStatus statusForm = new frmStatus(paths,
							chkVideo.Checked, chkAudio.Checked, chkTimeCodes.Checked, chkTranscode.Checked))
						{
							bool topMost = TopMost;
							TopMost = false;
							statusForm.ShowDialog(this);
							TopMost = topMost;
						}
					});
				});
				_statusThread.Start();
			}
		}

		private void frmMain_Load(object sender, EventArgs e) {
			LoadSettings();
		}

		private void frmMain_FormClosed(object sender, FormClosedEventArgs e) {
			SaveSettings();
		}
	}
}
