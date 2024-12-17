namespace JDP
{
    partial class frmTimeinfo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Tag");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTimeinfo));
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.dataViewer = new Be.Windows.Forms.HexBox();
            this.tagTreeView = new System.Windows.Forms.TreeView();
            this.lvTime = new System.Windows.Forms.ListView();
            this.ch_frameIdx = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_offset_hex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_tag_type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_tag_size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_pkgType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_codec = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_dts = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_dts_diff = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_pts = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_pts_minus_dts = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.detailTreeView = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.onlyVideoRatioButton = new System.Windows.Forms.RadioButton();
            this.onlyAudioRatioButton = new System.Windows.Forms.RadioButton();
            this.fileFramesRadioButton = new System.Windows.Forms.RadioButton();
            this.nextKeyFrameButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataViewer
            // 
            this.dataViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataViewer.ColumnInfoVisible = true;
            this.dataViewer.Font = new System.Drawing.Font("Monaco", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataViewer.HexCasing = Be.Windows.Forms.HexCasing.Lower;
            this.dataViewer.LineInfoVisible = true;
            this.dataViewer.Location = new System.Drawing.Point(3, 441);
            this.dataViewer.Name = "dataViewer";
            this.dataViewer.ReadOnly = true;
            this.dataViewer.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.dataViewer.Size = new System.Drawing.Size(900, 183);
            this.dataViewer.StringViewVisible = true;
            this.dataViewer.TabIndex = 0;
            this.dataViewer.UseFixedBytesPerLine = true;
            this.dataViewer.VScrollBarVisible = true;
            // 
            // tagTreeView
            // 
            this.tagTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tagTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tagTreeView.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tagTreeView.Location = new System.Drawing.Point(3, 379);
            this.tagTreeView.Name = "tagTreeView";
            treeNode1.Name = "Tag";
            treeNode1.Text = "Tag";
            this.tagTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.tagTreeView.Size = new System.Drawing.Size(379, 245);
            this.tagTreeView.TabIndex = 0;
            // 
            // lvTime
            // 
            this.lvTime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvTime.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ch_frameIdx,
            this.ch_offset_hex,
            this.ch_tag_type,
            this.ch_tag_size,
            this.ch_pkgType,
            this.ch_codec,
            this.ch_dts,
            this.ch_dts_diff,
            this.ch_pts,
            this.ch_pts_minus_dts});
            this.lvTime.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvTime.FullRowSelect = true;
            this.lvTime.GridLines = true;
            this.lvTime.HideSelection = false;
            this.lvTime.Location = new System.Drawing.Point(3, 3);
            this.lvTime.MultiSelect = false;
            this.lvTime.Name = "lvTime";
            this.lvTime.Size = new System.Drawing.Size(900, 432);
            this.lvTime.TabIndex = 0;
            this.lvTime.UseCompatibleStateImageBehavior = false;
            this.lvTime.View = System.Windows.Forms.View.Details;
            this.lvTime.VirtualListSize = 200;
            this.lvTime.VirtualMode = true;
            this.lvTime.CacheVirtualItems += new System.Windows.Forms.CacheVirtualItemsEventHandler(this.lvTime_CacheVirtualItems);
            this.lvTime.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvTime_ItemSelectionChanged);
            this.lvTime.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lvTime_RetrieveVirtualItem);
            this.lvTime.SearchForVirtualItem += new System.Windows.Forms.SearchForVirtualItemEventHandler(this.lvTime_SearchForVirtualItem);
            this.lvTime.VirtualItemsSelectionRangeChanged += new System.Windows.Forms.ListViewVirtualItemsSelectionRangeChangedEventHandler(this.lvTime_VirtualItemsSelectionRangeChanged);
            // 
            // ch_frameIdx
            // 
            this.ch_frameIdx.Text = "index";
            this.ch_frameIdx.Width = 80;
            // 
            // ch_offset_hex
            // 
            this.ch_offset_hex.Text = "offset";
            // 
            // ch_tag_type
            // 
            this.ch_tag_type.Text = "tag type";
            // 
            // ch_tag_size
            // 
            this.ch_tag_size.Text = "tag size";
            // 
            // ch_pkgType
            // 
            this.ch_pkgType.Text = "packet type";
            this.ch_pkgType.Width = 107;
            // 
            // ch_codec
            // 
            this.ch_codec.Text = "codec";
            // 
            // ch_dts
            // 
            this.ch_dts.Text = "dts";
            this.ch_dts.Width = 80;
            // 
            // ch_dts_diff
            // 
            this.ch_dts_diff.Text = "dts-dts";
            this.ch_dts_diff.Width = 80;
            // 
            // ch_pts
            // 
            this.ch_pts.Text = "pts";
            this.ch_pts.Width = 80;
            // 
            // ch_pts_minus_dts
            // 
            this.ch_pts_minus_dts.Text = "pts-dts";
            this.ch_pts_minus_dts.Width = 80;
            // 
            // detailTreeView
            // 
            this.detailTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.detailTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.detailTreeView.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detailTreeView.Location = new System.Drawing.Point(3, 3);
            this.detailTreeView.Name = "detailTreeView";
            this.detailTreeView.Size = new System.Drawing.Size(379, 370);
            this.detailTreeView.TabIndex = 0;
            this.detailTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.detailTreeView_NodeMouseClick);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 75);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1303, 633);
            this.tableLayoutPanel2.TabIndex = 6;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.dataViewer, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lvTime, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(906, 627);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.detailTreeView, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tagTreeView, 0, 1);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(915, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(385, 627);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // onlyVideoRatioButton
            // 
            this.onlyVideoRatioButton.AutoSize = true;
            this.onlyVideoRatioButton.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.onlyVideoRatioButton.Location = new System.Drawing.Point(106, 12);
            this.onlyVideoRatioButton.Name = "onlyVideoRatioButton";
            this.onlyVideoRatioButton.Size = new System.Drawing.Size(89, 49);
            this.onlyVideoRatioButton.TabIndex = 1;
            this.onlyVideoRatioButton.TabStop = true;
            this.onlyVideoRatioButton.Text = "📽️";
            this.onlyVideoRatioButton.UseVisualStyleBackColor = true;
            this.onlyVideoRatioButton.CheckedChanged += new System.EventHandler(this.onlyVideoRatioButton_CheckedChanged);
            // 
            // onlyAudioRatioButton
            // 
            this.onlyAudioRatioButton.AutoSize = true;
            this.onlyAudioRatioButton.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.onlyAudioRatioButton.Location = new System.Drawing.Point(195, 12);
            this.onlyAudioRatioButton.Name = "onlyAudioRatioButton";
            this.onlyAudioRatioButton.Size = new System.Drawing.Size(89, 49);
            this.onlyAudioRatioButton.TabIndex = 2;
            this.onlyAudioRatioButton.TabStop = true;
            this.onlyAudioRatioButton.Text = "🔊";
            this.onlyAudioRatioButton.UseVisualStyleBackColor = true;
            this.onlyAudioRatioButton.CheckedChanged += new System.EventHandler(this.onlyAudioFrames_CheckedChanged);
            // 
            // fileFramesRadioButton
            // 
            this.fileFramesRadioButton.AutoSize = true;
            this.fileFramesRadioButton.Checked = true;
            this.fileFramesRadioButton.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileFramesRadioButton.Location = new System.Drawing.Point(17, 12);
            this.fileFramesRadioButton.Name = "fileFramesRadioButton";
            this.fileFramesRadioButton.Size = new System.Drawing.Size(89, 49);
            this.fileFramesRadioButton.TabIndex = 3;
            this.fileFramesRadioButton.TabStop = true;
            this.fileFramesRadioButton.Text = "🎞️";
            this.fileFramesRadioButton.UseVisualStyleBackColor = true;
            this.fileFramesRadioButton.CheckedChanged += new System.EventHandler(this.fileFramesRadioButton_CheckedChanged);
            // 
            // nextKeyFrameButton
            // 
            this.nextKeyFrameButton.Location = new System.Drawing.Point(284, 12);
            this.nextKeyFrameButton.Name = "nextKeyFrameButton";
            this.nextKeyFrameButton.Size = new System.Drawing.Size(89, 49);
            this.nextKeyFrameButton.TabIndex = 4;
            this.nextKeyFrameButton.Text = "Next Key";
            this.nextKeyFrameButton.UseVisualStyleBackColor = true;
            this.nextKeyFrameButton.Click += new System.EventHandler(this.nextKeyFrameButton_Click);
            // 
            // frmTimeinfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1327, 720);
            this.Controls.Add(this.nextKeyFrameButton);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.fileFramesRadioButton);
            this.Controls.Add(this.onlyAudioRatioButton);
            this.Controls.Add(this.onlyVideoRatioButton);
            this.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximumSize = new System.Drawing.Size(2560, 1440);
            this.Name = "frmTimeinfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Time Information";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTimeinfo_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmTimeinfo_FormClosed);
            this.Load += new System.EventHandler(this.frmTimeinfo_Load);
            this.Shown += new System.EventHandler(this.frmTimeinfo_Shown);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Be.Windows.Forms.HexBox dataViewer;
        private System.Windows.Forms.TreeView tagTreeView;
        private System.Windows.Forms.ListView lvTime;
        private System.Windows.Forms.ColumnHeader ch_frameIdx;
        private System.Windows.Forms.ColumnHeader ch_offset_hex;
        private System.Windows.Forms.ColumnHeader ch_tag_type;
        private System.Windows.Forms.ColumnHeader ch_tag_size;
        private System.Windows.Forms.ColumnHeader ch_pkgType;
        private System.Windows.Forms.ColumnHeader ch_codec;
        private System.Windows.Forms.ColumnHeader ch_dts;
        private System.Windows.Forms.ColumnHeader ch_dts_diff;
        private System.Windows.Forms.ColumnHeader ch_pts;
        private System.Windows.Forms.ColumnHeader ch_pts_minus_dts;
        private System.Windows.Forms.TreeView detailTreeView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.RadioButton onlyVideoRatioButton;
        private System.Windows.Forms.RadioButton onlyAudioRatioButton;
        private System.Windows.Forms.RadioButton fileFramesRadioButton;
        private System.Windows.Forms.Button nextKeyFrameButton;
    }
}