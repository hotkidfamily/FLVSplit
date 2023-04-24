using System.ComponentModel.Design;

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
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Tag");
            this.lvTime = new System.Windows.Forms.ListView();
            this.ch_frameIdx = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_offset_hex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_tag_type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_tag_size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_dts = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_dts_diff = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_pts = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_pts_minus_dts = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tagDetailGroupBox = new System.Windows.Forms.GroupBox();
            this.tagTreeView = new System.Windows.Forms.TreeView();
            this.dataGroupBox = new System.Windows.Forms.GroupBox();
            this.dataViewer = new Be.Windows.Forms.HexBox();
            this.detailGroupBox = new System.Windows.Forms.GroupBox();
            this.detailTreeView = new System.Windows.Forms.TreeView();
            this.framesGroupBox = new System.Windows.Forms.GroupBox();
            this.fileFramesRadioButton = new System.Windows.Forms.RadioButton();
            this.onlyAudioRatioButton = new System.Windows.Forms.RadioButton();
            this.onlyVideoRatioButton = new System.Windows.Forms.RadioButton();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.ch_pkgType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nextKeyFrameButton = new System.Windows.Forms.Button();
            this.tagDetailGroupBox.SuspendLayout();
            this.dataGroupBox.SuspendLayout();
            this.detailGroupBox.SuspendLayout();
            this.framesGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvTime
            // 
            this.lvTime.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ch_frameIdx,
            this.ch_offset_hex,
            this.ch_tag_type,
            this.ch_tag_size,
            this.ch_pkgType,
            this.ch_dts,
            this.ch_dts_diff,
            this.ch_pts,
            this.ch_pts_minus_dts});
            this.lvTime.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvTime.FullRowSelect = true;
            this.lvTime.GridLines = true;
            this.lvTime.HideSelection = false;
            this.lvTime.Location = new System.Drawing.Point(6, 55);
            this.lvTime.MultiSelect = false;
            this.lvTime.Name = "lvTime";
            this.lvTime.Size = new System.Drawing.Size(880, 440);
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
            // tagDetailGroupBox
            // 
            this.tagDetailGroupBox.AutoSize = true;
            this.tagDetailGroupBox.Controls.Add(this.tagTreeView);
            this.tagDetailGroupBox.Location = new System.Drawing.Point(903, 516);
            this.tagDetailGroupBox.Name = "tagDetailGroupBox";
            this.tagDetailGroupBox.Size = new System.Drawing.Size(415, 363);
            this.tagDetailGroupBox.TabIndex = 3;
            this.tagDetailGroupBox.TabStop = false;
            this.tagDetailGroupBox.Text = "Tag Detail";
            // 
            // tagTreeView
            // 
            this.tagTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tagTreeView.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tagTreeView.Location = new System.Drawing.Point(6, 26);
            this.tagTreeView.Name = "tagTreeView";
            treeNode4.Name = "Tag";
            treeNode4.Text = "Tag";
            this.tagTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4});
            this.tagTreeView.Size = new System.Drawing.Size(403, 312);
            this.tagTreeView.TabIndex = 0;
            // 
            // dataGroupBox
            // 
            this.dataGroupBox.AutoSize = true;
            this.dataGroupBox.Controls.Add(this.dataViewer);
            this.dataGroupBox.Location = new System.Drawing.Point(0, 517);
            this.dataGroupBox.Name = "dataGroupBox";
            this.dataGroupBox.Size = new System.Drawing.Size(897, 362);
            this.dataGroupBox.TabIndex = 4;
            this.dataGroupBox.TabStop = false;
            this.dataGroupBox.Text = "Data";
            // 
            // dataViewer
            // 
            this.dataViewer.ColumnInfoVisible = true;
            this.dataViewer.Font = new System.Drawing.Font("Monaco", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataViewer.HexCasing = Be.Windows.Forms.HexCasing.Lower;
            this.dataViewer.LineInfoVisible = true;
            this.dataViewer.Location = new System.Drawing.Point(6, 26);
            this.dataViewer.Name = "dataViewer";
            this.dataViewer.ReadOnly = true;
            this.dataViewer.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.dataViewer.Size = new System.Drawing.Size(880, 311);
            this.dataViewer.StringViewVisible = true;
            this.dataViewer.TabIndex = 0;
            this.dataViewer.UseFixedBytesPerLine = true;
            this.dataViewer.VScrollBarVisible = true;
            // 
            // detailGroupBox
            // 
            this.detailGroupBox.AutoSize = true;
            this.detailGroupBox.Controls.Add(this.detailTreeView);
            this.detailGroupBox.Location = new System.Drawing.Point(902, 0);
            this.detailGroupBox.Name = "detailGroupBox";
            this.detailGroupBox.Size = new System.Drawing.Size(416, 520);
            this.detailGroupBox.TabIndex = 4;
            this.detailGroupBox.TabStop = false;
            this.detailGroupBox.Text = "Detail";
            // 
            // detailTreeView
            // 
            this.detailTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.detailTreeView.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detailTreeView.Location = new System.Drawing.Point(7, 26);
            this.detailTreeView.Name = "detailTreeView";
            this.detailTreeView.Size = new System.Drawing.Size(403, 469);
            this.detailTreeView.TabIndex = 0;
            this.detailTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.detailTreeView_NodeMouseClick);
            // 
            // framesGroupBox
            // 
            this.framesGroupBox.AutoSize = true;
            this.framesGroupBox.Controls.Add(this.nextKeyFrameButton);
            this.framesGroupBox.Controls.Add(this.fileFramesRadioButton);
            this.framesGroupBox.Controls.Add(this.onlyAudioRatioButton);
            this.framesGroupBox.Controls.Add(this.onlyVideoRatioButton);
            this.framesGroupBox.Controls.Add(this.lvTime);
            this.framesGroupBox.Location = new System.Drawing.Point(0, 0);
            this.framesGroupBox.Name = "framesGroupBox";
            this.framesGroupBox.Size = new System.Drawing.Size(897, 520);
            this.framesGroupBox.TabIndex = 5;
            this.framesGroupBox.TabStop = false;
            this.framesGroupBox.Text = "Frames";
            // 
            // fileFramesRadioButton
            // 
            this.fileFramesRadioButton.AutoSize = true;
            this.fileFramesRadioButton.Checked = true;
            this.fileFramesRadioButton.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileFramesRadioButton.Location = new System.Drawing.Point(11, 15);
            this.fileFramesRadioButton.Name = "fileFramesRadioButton";
            this.fileFramesRadioButton.Size = new System.Drawing.Size(60, 34);
            this.fileFramesRadioButton.TabIndex = 3;
            this.fileFramesRadioButton.TabStop = true;
            this.fileFramesRadioButton.Text = "🎞️";
            this.fileFramesRadioButton.UseVisualStyleBackColor = true;
            this.fileFramesRadioButton.CheckedChanged += new System.EventHandler(this.fileFramesRadioButton_CheckedChanged);
            // 
            // onlyAudioRatioButton
            // 
            this.onlyAudioRatioButton.AutoSize = true;
            this.onlyAudioRatioButton.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.onlyAudioRatioButton.Location = new System.Drawing.Point(149, 15);
            this.onlyAudioRatioButton.Name = "onlyAudioRatioButton";
            this.onlyAudioRatioButton.Size = new System.Drawing.Size(60, 34);
            this.onlyAudioRatioButton.TabIndex = 2;
            this.onlyAudioRatioButton.TabStop = true;
            this.onlyAudioRatioButton.Text = "🔊";
            this.onlyAudioRatioButton.UseVisualStyleBackColor = true;
            this.onlyAudioRatioButton.CheckedChanged += new System.EventHandler(this.onlyAudioFrames_CheckedChanged);
            // 
            // onlyVideoRatioButton
            // 
            this.onlyVideoRatioButton.AutoSize = true;
            this.onlyVideoRatioButton.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.onlyVideoRatioButton.Location = new System.Drawing.Point(80, 15);
            this.onlyVideoRatioButton.Name = "onlyVideoRatioButton";
            this.onlyVideoRatioButton.Size = new System.Drawing.Size(60, 34);
            this.onlyVideoRatioButton.TabIndex = 1;
            this.onlyVideoRatioButton.TabStop = true;
            this.onlyVideoRatioButton.Text = "📽️";
            this.onlyVideoRatioButton.UseVisualStyleBackColor = true;
            this.onlyVideoRatioButton.CheckedChanged += new System.EventHandler(this.onlyVideoRatioButton_CheckedChanged);
            // 
            // ch_pkgType
            // 
            this.ch_pkgType.Text = "packet type";
            // 
            // nextKeyFrameButton
            // 
            this.nextKeyFrameButton.Location = new System.Drawing.Point(215, 15);
            this.nextKeyFrameButton.Name = "nextKeyFrameButton";
            this.nextKeyFrameButton.Size = new System.Drawing.Size(87, 34);
            this.nextKeyFrameButton.TabIndex = 4;
            this.nextKeyFrameButton.Text = "Next Key";
            this.nextKeyFrameButton.UseVisualStyleBackColor = true;
            this.nextKeyFrameButton.Click += new System.EventHandler(this.nextKeyFrameButton_Click);
            // 
            // frmTimeinfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1327, 887);
            this.Controls.Add(this.tagDetailGroupBox);
            this.Controls.Add(this.framesGroupBox);
            this.Controls.Add(this.detailGroupBox);
            this.Controls.Add(this.dataGroupBox);
            this.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmTimeinfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Time Information";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTimeinfo_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmTimeinfo_FormClosed);
            this.Load += new System.EventHandler(this.frmTimeinfo_Load);
            this.Shown += new System.EventHandler(this.frmTimeinfo_Shown);
            this.tagDetailGroupBox.ResumeLayout(false);
            this.dataGroupBox.ResumeLayout(false);
            this.detailGroupBox.ResumeLayout(false);
            this.framesGroupBox.ResumeLayout(false);
            this.framesGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvTime;
        private System.Windows.Forms.ColumnHeader ch_dts;
        private System.Windows.Forms.ColumnHeader ch_dts_diff;
        private System.Windows.Forms.ColumnHeader ch_pts;
        private System.Windows.Forms.ColumnHeader ch_pts_minus_dts;
        private System.Windows.Forms.ColumnHeader ch_frameIdx;
        private System.Windows.Forms.ColumnHeader ch_offset_hex;
        private System.Windows.Forms.ColumnHeader ch_tag_type;
        private System.Windows.Forms.ColumnHeader ch_tag_size;
        private System.Windows.Forms.GroupBox tagDetailGroupBox;
        private System.Windows.Forms.GroupBox dataGroupBox;
        private System.Windows.Forms.GroupBox detailGroupBox;
        private System.Windows.Forms.GroupBox framesGroupBox;
        private System.Windows.Forms.TreeView tagTreeView;
        private System.Windows.Forms.TreeView detailTreeView;
        private Be.Windows.Forms.HexBox dataViewer;
        private System.Windows.Forms.RadioButton onlyVideoRatioButton;
        private System.Windows.Forms.RadioButton onlyAudioRatioButton;
        private System.Windows.Forms.RadioButton fileFramesRadioButton;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ColumnHeader ch_pkgType;
        private System.Windows.Forms.Button nextKeyFrameButton;
    }
}