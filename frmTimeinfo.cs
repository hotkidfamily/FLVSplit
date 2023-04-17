using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JDP
{
    public partial class frmTimeinfo : Form
    {
        private Thread _workThread;
        List<TimeInfo> _records;

        public frmTimeinfo(List<TimeInfo> records)
        {
            _records = records;
            InitializeComponent();
        }

        private void frmTimeinfo_Shown(object sender, EventArgs e)
        {
            Activate();

            _workThread = new Thread(ExtractTimeThread);
            _workThread.Start();
        }
        private void frmTimeinfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((_workThread != null) && _workThread.IsAlive)
            {
                e.Cancel = true;
            }
        }

        private void ExtractTimeThread()
        {
            ListViewItem item = null;

            for (int i = 0; i < _records.Count(); i++)
            {
                Invoke((MethodInvoker)delegate () { 
                    item = lvTime.Items.Add(new ListViewItem(new string[] { _records[i].dts.ToString(), 
                        _records[i].dtsStep.ToString(), _records[i].pts.ToString(), _records[i].composTime.ToString(), }));
                    item.EnsureVisible();
                });
            }
        }

    }
}
