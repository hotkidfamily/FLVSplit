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

        class TimeinfoMap : ClassMap<TimeInfo>
        {
            public TimeinfoMap()
            {
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
                _items.Add(new ListViewItem(new string[] { i.ToString(), " ", _records[i].dts,
                        _records[i].dtsStep, _records[i].pts, _records[i].composTime, }));
            }

            InitializeComponent();
        }

        private void frmTimeinfo_Shown(object sender, EventArgs e)
        {
            Activate();
            lvTime.VirtualListSize = _records.Count;
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
