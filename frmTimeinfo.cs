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

using System.ComponentModel.Design;
using Be.Windows.Forms;
using System.Xaml;

namespace JDP
{

    public partial class frmTimeinfo : Form
    {
        string _binPath = string.Empty;
        List<TimeInfo> _records = null;
        private List<ListViewItem> _items = null;
        public class TimeInfo
        {
            public string indx { get; set; }
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
                Map(m => m.indx).Name("frames");
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

            try
            {
                using (var reader = new StreamReader(csvPath))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Context.RegisterClassMap<TimeinfoMap>();
                    _records = csv.GetRecords<TimeInfo>().ToList();
                }
            }
            catch (Exception ex)
            {
                _records = null;
            }

            if (_records == null || _records.Count == 0) 
            {
                _records = new List<TimeInfo>();
                _records.Add(new TimeInfo() { indx = "-1", offset = "-1", tagType = "x", tagSize = "-1", dts = "-1", dtsStep = "-1", pts = "-1", composTime = "-1" });
            }

            _items = new List<ListViewItem>();
            for (int i = 0; i < _records.Count(); i++)
            {
                var v = new ListViewItem(new string[] { _records[i].indx, _records[i].offset, _records[i].tagType == "9" ? "📽":"🔊", _records[i].tagSize, _records[i].dts,
                    _records[i].dtsStep, _records[i].pts, _records[i].composTime}) ;
                _items.Add(v);
                
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
        private void FillTagTreeView(ref FlvTag tag)
        {
            tagTreeView.BeginUpdate();

            tagTreeView.Nodes.Clear();

            TreeNode root = new TreeNode("FLVTag");
            root.NodeFont = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            root.Nodes.Add("Tag Type: " + tag.tagType); 
            root.Nodes.Add("Data Size: " + tag.dataSize);
            root.Nodes.Add("Timestamp: " + tag.timestamp);
            root.Nodes.Add("Stream ID: " + tag.streamID);

            if (tag.v.frametype != null)
            {
                TreeNode vNode = new TreeNode("Video");
                vNode.NodeFont = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                vNode.Nodes.Add("FrameType: " + tag.v.frametype);
                vNode.Nodes.Add("Codec ID: " + tag.v.codecID);
                vNode.Nodes.Add("AVC Packet Type: " + tag.v.avcPacketType);
                vNode.Nodes.Add("Composition Time: " + tag.v.compositionTime);
                root.Nodes.Add(vNode);
            }

            if (tag.a.soundFormat != null)
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

            root.Nodes.Add("Preview Tag Size: " + tag.previousTagSize);
            tagTreeView.Nodes.Add(root);
            tagTreeView.ExpandAll();
            tagTreeView.EndUpdate();
        }
        private void FillDetailTreeView(ref FlvTag tag)
        {
            detailTreeView.BeginUpdate();
            detailTreeView.Nodes.Clear();

            TreeNode root = new TreeNode();
            if (tag.v.frametype != null)
            {
                root = new TreeNode("VideoData");
                root.NodeFont = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                root.Nodes.Add("Nalus Size: " + tag.v.NALUs);
                for (int i = 0; i<tag.v.NALUs; i++)
                {
                    var v = tag.v.NaluDetails[i];
                    TreeNode leaf = new TreeNode("Nalu");
                    leaf.NodeFont = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                    leaf.Nodes.Add("type: " + v.type);
                    leaf.Nodes.Add("offset: " + v.offset);
                    leaf.Tag = new NaluDetail() { type = v.type, offset = v.offset };
                    root.Nodes.Add(leaf);
                }
            }
            detailTreeView.Nodes.Add(root);
            detailTreeView.ExpandAll();
            detailTreeView.EndUpdate();
        }
        private void FillBinaryDataView(ref FlvTag tag)
        {
            if (tag.data != null)
            {
                DynamicByteProvider provider = new DynamicByteProvider(tag.data);
                dataViewer.ByteProvider = provider;
            }
        }

        private void lvTime_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                FlvSpecs flvSpecs = new FlvSpecs(_binPath);
                int idx = int.Parse(e.Item.Text) - 1;

                {
                    var c = _records[idx];
                    long offset = long.Parse(c.offset);
                    if(offset != -1)
                    {
                        FlvTag tag = new FlvTag();
                        flvSpecs.parseTag(offset, ref tag);
                        FillTagTreeView(ref tag);
                        FillDetailTreeView(ref tag);
                        FillBinaryDataView(ref tag);
                    }
                }
            }
        }

        private void frmTimeinfo_Load(object sender, EventArgs e)
        {

        }

        private void onlyVideoRatioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (onlyVideoRatioButton.Checked)
            {
                _items = new List<ListViewItem>();
                for (int i = 0; i < _records.Count(); i++)
                {
                    if (_records[i].tagType == "9")
                    {
                        var v = new ListViewItem(new string[] { _records[i].indx, _records[i].offset, "📽", _records[i].tagSize, _records[i].dts,
                    _records[i].dtsStep, _records[i].pts, _records[i].composTime});
                        _items.Add(v);
                    }
                }
                lvTime.VirtualListSize = _items.Count;
                lvTime.Refresh();
            }
        }

        private void onlyAudioFrames_CheckedChanged(object sender, EventArgs e)
        {
            if (onlyAudioRatioButton.Checked)
            {
                _items = new List<ListViewItem>();
                for (int i = 0; i < _records.Count(); i++)
                {
                    if (_records[i].tagType == "8")
                    {
                        var v = new ListViewItem(new string[] { _records[i].indx, _records[i].offset, "🔊", _records[i].tagSize, _records[i].dts,
                    _records[i].dtsStep, _records[i].pts, _records[i].composTime});
                        _items.Add(v);
                    }
                }
                lvTime.VirtualListSize = _items.Count;
                lvTime.Refresh();
            }
        }

        private void fileFramesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fileFramesRadioButton.Checked)
            {
                _items = new List<ListViewItem>();
                for (int i = 0; i < _records.Count(); i++)
                {
                    var v = new ListViewItem(new string[] { _records[i].indx, _records[i].offset, _records[i].tagType == "9" ? "📽":"🔊", _records[i].tagSize, _records[i].dts,
                    _records[i].dtsStep, _records[i].pts, _records[i].composTime});
                    _items.Add(v);
                }
                lvTime.VirtualListSize = _items.Count;
                lvTime.Refresh();
            }
        }

        private void detailTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode currentNode = e.Node;
            if(currentNode != null && currentNode.Parent != null)
            {
                var n = currentNode.Parent;
                if (n.Tag != null)
                {
                    NaluDetail v = n.Tag as NaluDetail;
                    if (v != null)
                    {
                        dataViewer.Select(v.offset, 1);
                    }
                }
            }
        }
    }
}
