using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using JDP.Library;
using System.Text;
using System.Drawing;

namespace JDP
{

    public partial class frmTimeinfo : Form
    {
        string _binPath = string.Empty;
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


        public frmTimeinfo(string binPath)
        {
            _binPath = binPath;

            string csvPath = Path.Combine(Path.GetDirectoryName(binPath), Path.GetFileNameWithoutExtension(binPath));
            csvPath = csvPath + ".txt";
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
                _items.Add(new ListViewItem(new string[] { i.ToString(), _records[i].offset, _records[i].tagType == "9" ? "📽":"🔈", _records[i].tagSize, _records[i].dts,
                        _records[i].dtsStep, _records[i].pts, _records[i].composTime, }));
            }

            InitializeComponent();
        }

        private void frmTimeinfo_Shown(object sender, EventArgs e)
        {
            Activate();
            lvTime.VirtualListSize = _records.Count;
            int initialWidth = lvTime.Width;
            Program.SetFontAndScaling(this);
            int v = Convert.ToInt32(initialWidth / (lvTime.Columns.Count + 1));
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

        private void lvTime_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                FlvSpecs flvSpecs = new FlvSpecs(_binPath);
                long offset = long.Parse(_records[e.ItemIndex].offset);
                FlvTag tag = new FlvTag();
                flvSpecs.parseTag(offset, ref tag);

                string result = String.Format("Tag = {0}\r\nDataSize = {1}\r\nTimestamp = {2}\r\nstreamID = {3}\r\n", 
                    tag.tagType == 8 ? "audio" : "video", tag.dataSize, tag.timestamp, tag.streamID);

                if(tag.tagType == 9)
                {
                    string vd = String.Format("📽️FrameType = {0}\r\n📽️CodecID = {1}\r\n📽️PacketType = {2}\r\n📽️Composition = {3}\r\n",
                        tag.v.frametype, tag.v.codecID, tag.v.avcPacketType, tag.v.compositionTime);
                    tagEditbox.Text = result + vd;
                }
                else
                {
                    string ad = String.Format("🔈Format = {0}\r\n🔈Rate = {1}\r\n🔈Size = {2}\r\n🔈Type = {3}\r\n🔈PacketType = {4}",
                        tag.a.soundFormat, tag.a.soundRate, tag.a.soundSize, tag.a.soundType, tag.a.aacPacketType);
                    tagEditbox.Text = result + ad;
                }
                string previousTagSize = String.Format("PreviousTagSize = {0}\r\n", tag.previousTagSize);
                tagEditbox.Text += previousTagSize;

                if (tag.data != null) {
                    codecEditbox.Clear();
                    string nal = BitConverter.ToString(tag.data, 0, Math.Min(tag.data.Count<byte>(), 72));

                    string tagText = nal.Substring(0, 33);
                    string videoTagText = nal.Substring(33, 14);
                    string nalText = nal.Substring(47, nal.Length - 47);

                    // 构造Rtf格式的文本
                    string rtfText = "{\\rtf1\\ansi\\deff0 {\\colortbl;\\red255\\green0\\blue0;\\red0\\green200\\blue40;\\red0\\green0\\blue0;}"; // 开始标记，定义颜色表
                    rtfText += $"{{\\cf1 {tagText}}}";
                    rtfText += $"{{\\cf2 {videoTagText}}}";
                    rtfText += $"{{\\cf0 {nalText}}}";
                    rtfText += "}"; // 结束标记

                    // 在RichTextBox控件中显示Rtf格式的文本
                    codecEditbox.Rtf = rtfText;
                }
            }
        }
    }
}
