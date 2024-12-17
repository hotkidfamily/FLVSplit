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
            public int indx { get; set; }
            public int offset { get; set; }
            public int tagType { get; set; }
            public int tagSize { get; set; }
            public int pkgType { get; set; }
            public int codecType { get; set; }
            public int dts { get; set; }
            public int dtsStep { get; set; }
            public int pts { get; set; }
            public int composTime { get; set; }

            public TimeInfo()
            {
                indx = -1;
                offset = -1;
                tagType = -1;
                tagSize = -1;
                pkgType = -1;
                codecType = -1;
                dts = -1;
                dtsStep = -1;
                pts = -1;
                composTime = -1;
            }
        }

        class TimeinfoMap : ClassMap<TimeInfo>
        {
            public TimeinfoMap()
            {
                Map(m => m.indx).Name("frames");
                Map(m => m.offset).Name("offset");
                Map(m => m.tagType).Name("tagType");
                Map(m => m.tagSize).Name("tagSize");
                Map(m => m.pkgType).Name("pkgType");
                Map(m => m.codecType).Name("codecType");
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
            catch (Exception)
            {
                _records = null;
            }

            if (_records == null || _records.Count == 0) 
            {
                _records = new List<TimeInfo>() { new TimeInfo()};
            }

            _items = new List<ListViewItem>();
            for (int i = 0; i < _records.Count(); i++)
            {
                var record = _records[i];
                var v = _makeListViewItem(ref record);
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
                var item = e.Item;
                TimeInfo ti = null;
                if (e.Item.Tag != null)
                    ti = e.Item.Tag as TimeInfo;

                if ( ti != null && ti.offset != -1)
                {
                    FlvSpecs flvSpecs = new FlvSpecs(_binPath);

                    FlvTag tag = new FlvTag();
                    flvSpecs.parseTag(ti.offset, ref tag);
                    FillTagTreeView(ref tag);
                    FillDetailTreeView(ref tag);
                    FillBinaryDataView(ref tag);
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
                nextKeyFrameButton.Enabled = true;

                _items = new List<ListViewItem>();
                for (int i = 0; i < _records.Count(); i++)
                {
                    if (_records[i].tagType == 9)
                    {
                        var record = _records[i];
                        var v = _makeListViewItem(ref record);
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
                nextKeyFrameButton.Enabled = false;
                _items = new List<ListViewItem>();
                for (int i = 0; i < _records.Count(); i++)
                {
                    if (_records[i].tagType == 8)
                    {
                        var record = _records[i];
                        var v = _makeListViewItem(ref record);
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
                nextKeyFrameButton.Enabled = true;
                _items = new List<ListViewItem>();
                for (int i = 0; i < _records.Count(); i++)
                {
                    var record = _records[i];
                    var v = _makeListViewItem(ref record);
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

        private void nextKeyFrameButton_Click(object sender, EventArgs e)
        {
            int idx = 0;
            if(lvTime.SelectedIndices.Count > 0)
            {
                idx = lvTime.SelectedIndices[0];
            }

            if(_items.Count > idx) {
                var nidx = _items.FindIndex(idx + 1, elem => (elem.Tag as TimeInfo)?.pkgType == 1 && elem.Tag != null);
                if (nidx != -1)
                {
                    lvTime.Items[nidx].Selected = true;
                    lvTime.Items[nidx].Focused = true;
                    lvTime.Items[nidx].EnsureVisible();
                }
            }

        }

        private ListViewItem _makeListViewItem(ref TimeInfo record)
        {
            string codecid;
            string packetType;
            string tagType;
            if (record.tagType == 9)
            {
                codecid = FlvSpecs.strVideoCodecID((uint)record.codecType);
                packetType = FlvSpecs.strVideoTagFrameType((uint)record.pkgType);
                tagType = "📽";
            }
            else
            {
                codecid = FlvSpecs.strSoundFormat((uint)record.codecType);
                packetType = "🔊";
                tagType = "🔊";
            }

            var v = new ListViewItem(new string[] {
                    record.indx.ToString(), record.offset.ToString(), tagType, record.tagSize.ToString(),
                    packetType, codecid, record.dts.ToString(), record.dtsStep.ToString(), record.pts.ToString(), record.composTime.ToString()});

            v.Tag = record;

            return v;
        }

	}
}
