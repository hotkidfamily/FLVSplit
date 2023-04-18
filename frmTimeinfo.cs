using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace JDP
{

    public partial class frmTimeinfo : Form
    {
        List<TimeInfo> _records = null;
        private List<ListViewItem> _items = null;
        public class TimeInfo
        {
            public string offset { get; set; }
            public string tagType { get; set; }
            public string tagSize { get; set; }
            public string dts { get; set; }
            public string dtsStep { get; set; }
            public string pts { get; set; }
            public string composTime { get; set; }
        }

        class TimeinfoMap : ClassMap<TimeInfo>
        {
            public TimeinfoMap()
            {
                Map(m => m.offset).Name("offset");
                Map(m => m.tagType).Name("tagType");
                Map(m => m.tagSize).Name("tagSize");
                Map(m => m.dts).Name("dts");
                Map(m => m.dtsStep).Name("dts-step");
                Map(m => m.pts).Name("pts");
                Map(m => m.composTime).Name("pts-dts");
            }
        }


        public frmTimeinfo(string csvPath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                AllowComments = true,
                Comment = '#',
            };

            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<TimeinfoMap>();
                _records = csv.GetRecords<TimeInfo>().ToList();
            }

            _items = new List<ListViewItem>();
            for (int i = 0; i < _records.Count(); i++)
            {
                _items.Add(new ListViewItem(new string[] { i.ToString(), _records[i].offset, _records[i].tagType, _records[i].tagSize, _records[i].dts,
                        _records[i].dtsStep, _records[i].pts, _records[i].composTime, }));
            }

            InitializeComponent();
        }

        private void frmTimeinfo_Shown(object sender, EventArgs e)
        {
            Activate();
            lvTime.VirtualListSize = _records.Count;
            int initialWidth = ClientSize.Width;
            Program.SetFontAndScaling(this);
            float scaleFactorX = (float)ClientSize.Width / initialWidth;
            int v = Convert.ToInt32(initialWidth * scaleFactorX / (lvTime.Columns.Count + 1));
            foreach (ColumnHeader columnHeader in lvTime.Columns)
            {
                columnHeader.Width = v;
            }
            lvTime.Items[0].Selected = true;
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
