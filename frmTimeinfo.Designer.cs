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
            this.lvTime = new System.Windows.Forms.ListView();
            this.ch_frameIdx = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_offset_hex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_tag_type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_tag_size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_dts = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_dts_diff = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_pts = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_pts_minus_dts = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.codecEditbox = new System.Windows.Forms.RichTextBox();
            this.tagEditbox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.lvTime.Location = new System.Drawing.Point(226, 11);
            this.lvTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lvTime.Name = "lvTime";
            this.lvTime.Size = new System.Drawing.Size(804, 651);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tagEditbox);
            this.groupBox1.Location = new System.Drawing.Point(12, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(207, 651);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tag";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.codecEditbox);
            this.groupBox2.Location = new System.Drawing.Point(1037, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(237, 650);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data";
            // 
            // codecEditbox
            // 
            this.codecEditbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.codecEditbox.Location = new System.Drawing.Point(6, 25);
            this.codecEditbox.Name = "codecEditbox";
            this.codecEditbox.Size = new System.Drawing.Size(225, 608);
            this.codecEditbox.TabIndex = 0;
            this.codecEditbox.Text = "";
            // 
            // tagEditbox
            // 
            this.tagEditbox.Location = new System.Drawing.Point(6, 26);
            this.tagEditbox.Multiline = true;
            this.tagEditbox.Name = "tagEditbox";
            this.tagEditbox.Size = new System.Drawing.Size(195, 608);
            this.tagEditbox.TabIndex = 0;
            // 
            // frmTimeinfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 666);
            this.Controls.Add(this.lvTime);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmTimeinfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Time Information";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTimeinfo_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmTimeinfo_FormClosed);
            this.Shown += new System.EventHandler(this.frmTimeinfo_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox codecEditbox;
        private System.Windows.Forms.TextBox tagEditbox;
    }
}