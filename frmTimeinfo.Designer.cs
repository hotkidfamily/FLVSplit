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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Tag");
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
            this.detailGroupBox = new System.Windows.Forms.GroupBox();
            this.detailTreeView = new System.Windows.Forms.TreeView();
            this.framesGroupBox = new System.Windows.Forms.GroupBox();
            this.tagDetailGroupBox.SuspendLayout();
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
            this.ch_dts,
            this.ch_dts_diff,
            this.ch_pts,
            this.ch_pts_minus_dts});
            this.lvTime.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvTime.FullRowSelect = true;
            this.lvTime.GridLines = true;
            this.lvTime.HideSelection = false;
            this.lvTime.Location = new System.Drawing.Point(6, 25);
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
            this.ch_tag_type.Text = "Tag Type";
            // 
            // ch_tag_size
            // 
            this.ch_tag_size.Text = "Tag Size";
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
            this.tagDetailGroupBox.Controls.Add(this.tagTreeView);
            this.tagDetailGroupBox.Location = new System.Drawing.Point(903, 477);
            this.tagDetailGroupBox.Name = "tagDetailGroupBox";
            this.tagDetailGroupBox.Size = new System.Drawing.Size(415, 337);
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
            treeNode1.Name = "Tag";
            treeNode1.Text = "Tag";
            this.tagTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.tagTreeView.Size = new System.Drawing.Size(403, 299);
            this.tagTreeView.TabIndex = 0;
            // 
            // dataGroupBox
            // 
            this.dataGroupBox.Location = new System.Drawing.Point(0, 478);
            this.dataGroupBox.Name = "dataGroupBox";
            this.dataGroupBox.Size = new System.Drawing.Size(897, 336);
            this.dataGroupBox.TabIndex = 4;
            this.dataGroupBox.TabStop = false;
            this.dataGroupBox.Text = "Data";
            // 
            // detailGroupBox
            // 
            this.detailGroupBox.Controls.Add(this.detailTreeView);
            this.detailGroupBox.Location = new System.Drawing.Point(903, 0);
            this.detailGroupBox.Name = "detailGroupBox";
            this.detailGroupBox.Size = new System.Drawing.Size(415, 471);
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
            this.detailTreeView.Size = new System.Drawing.Size(403, 439);
            this.detailTreeView.TabIndex = 0;
            // 
            // framesGroupBox
            // 
            this.framesGroupBox.Controls.Add(this.lvTime);
            this.framesGroupBox.Location = new System.Drawing.Point(0, 0);
            this.framesGroupBox.Name = "framesGroupBox";
            this.framesGroupBox.Size = new System.Drawing.Size(897, 471);
            this.framesGroupBox.TabIndex = 5;
            this.framesGroupBox.TabStop = false;
            this.framesGroupBox.Text = "Frames";
            // 
            // frmTimeinfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1324, 821);
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
            this.detailGroupBox.ResumeLayout(false);
            this.framesGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

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
        private System.ComponentModel.Design.ByteViewer dataHexviewer;
    }
}