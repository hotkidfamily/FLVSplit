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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace JDP
{
    public partial class frmTimeinfo : Form
    {
        List<TimeInfo> _records = null;
        private List<ListViewItem> _items = null;

        public frmTimeinfo(ref List<TimeInfo> records)
        {
            _records = records;
            _items = new List<ListViewItem>();
            for (int i = 0; i < _records.Count(); i++)
            {
                _items.Add(new ListViewItem(new string[] { i.ToString(), " ", _records[i].dts,
                        _records[i].dtsStep, _records[i].pts, _records[i].composTime, }));
            }

            InitializeComponent();
        }

        private void frmTimeinfo_Shown(object sender, EventArgs e)
        {
            Activate();
            lvTime.VirtualListSize = _records.Count;
        }
        private void frmTimeinfo_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void frmTimeinfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            _items = null;
        }

        private void lvTime_VirtualItemsSelectionRangeChanged(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e)
        {
        }

        private void lvTime_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if ((_items != null) && (e.ItemIndex <= _items.Count))
            {
                e.Item = _items[e.ItemIndex];
            }
            else { 
                e.Item = new ListViewItem(); 
            }
        }

        private void lvTime_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {

        }

        private void lvTime_SearchForVirtualItem(object sender, SearchForVirtualItemEventArgs e)
        {

        }

    }
}
