using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using JDP.Library;
using System.Drawing;

using WpfHexaEditor;
using WpfHexaEditor.Core;
using System.ComponentModel.Design;

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
            ByteViewer bv = new ByteViewer();
            bv.Name = "dataByteViewer";
            bv.TabStop = false;
            bv.SetColumn(bv, 16);
            bv.Font = new System.Drawing.Font("Segoe UI", 11.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

            bv.SetDisplayMode(DisplayMode.Hexdump);
            bv.SetColumnSpan(bv, 5);
            bv.VerticalScroll.Enabled = true;
            bv.VerticalScroll.Visible = true;
            bv.Dock = DockStyle.Fill; 
            dataHexviewer = bv;
            dataGroupBox.Controls.Add(bv);
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
        private void FillTagTreeView(ref FlvTag tag)
        {
            tagTreeView.Nodes.Clear();

            TreeNode root = new TreeNode("FLVTag");
            root.NodeFont = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            root.Nodes.Add("Tag Type: " + tag.tagType); 
            root.Nodes.Add("Data Size: " + tag.dataSize);
            root.Nodes.Add("Timestamp: " + tag.timestamp);
            root.Nodes.Add("Stream ID: " + tag.streamID);

            if (tag.v.frametype != "")
            {
                TreeNode vNode = new TreeNode("Video");
                vNode.NodeFont = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                vNode.Nodes.Add("FrameType: " + tag.v.frametype);
                vNode.Nodes.Add("Codec ID: " + tag.v.codecID);
                vNode.Nodes.Add("AVC Packet Type: " + tag.v.avcPacketType);
                vNode.Nodes.Add("Composition Time: " + tag.v.compositionTime);
                root.Nodes.Add(vNode);
            }

            if (tag.a.soundFormat != 0)
            {
                TreeNode aNode = new TreeNode("Audio");
                aNode.NodeFont = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                aNode.Nodes.Add("Format: " + tag.a.soundFormat);
                aNode.Nodes.Add("Rate: " + tag.a.soundRate);
                aNode.Nodes.Add("Size: " + tag.a.soundSize);
                aNode.Nodes.Add("Channel: " + tag.a.soundType);
                aNode.Nodes.Add("AAC Packet Type: " + tag.a.aacPacketType);
                root.Nodes.Add(aNode);
            }

/*            if (tag.data != null)
            {
                TreeNode dNode = new TreeNode("Data");
                dNode.Nodes.Add("Data: " + BitConverter.ToString(tag.data));
                root.Nodes.Add(dNode);
            }*/

            root.Nodes.Add("Preview Tag Size: " + tag.previousTagSize);
            tagTreeView.Nodes.Add(root);
            tagTreeView.ExpandAll();
        }
        private void FillDetailTreeView(ref FlvTag tag)
        {
            detailTreeView.Nodes.Clear();

            TreeNode root = new TreeNode();
            if (tag.v.frametype != "")
            {
                root = new TreeNode("VideoData");
                root.NodeFont = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                root.Nodes.Add("Nalus: " + tag.v.NALUs);
            }
            detailTreeView.Nodes.Add(root);
            detailTreeView.ExpandAll();
        }
        private void FillBinaryDataView(ref FlvTag tag)
        {
            WpfHexaEditor.HexEditor editor = new WpfHexaEditor.HexEditor();
            if (tag.data != null)
            {
                //codecEditbox.Clear();
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
                //codecEditbox.Rtf = rtfText;

                dataHexviewer.ResetText();
                dataHexviewer.SetBytes(tag.data);
            }
        }

        private void lvTime_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                FlvSpecs flvSpecs = new FlvSpecs(_binPath);
                long offset = long.Parse(_records[e.ItemIndex].offset);
                FlvTag tag = new FlvTag();
                flvSpecs.parseTag(offset, ref tag);
                FillTagTreeView(ref tag);
                FillDetailTreeView(ref tag);
                FillBinaryDataView(ref tag);
            }
        }

        private void frmTimeinfo_Load(object sender, EventArgs e)
        {

        }
    }
}
