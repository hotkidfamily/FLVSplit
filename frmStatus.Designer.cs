namespace JDP {
    partial class frmStatus {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStatus));
            this.lvStatus = new System.Windows.Forms.ListView();
            this.chStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chTrueFrameRate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAverageFrameRate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chGuessFrameRate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDetails = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCopyFrameRates = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnShowTimestamp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvStatus
            // 
            this.lvStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvStatus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chStatus,
            this.chFile,
            this.chTrueFrameRate,
            this.chAverageFrameRate,
            this.chGuessFrameRate,
            this.chDetails});
            this.lvStatus.FullRowSelect = true;
            this.lvStatus.HideSelection = false;
            this.lvStatus.Location = new System.Drawing.Point(8, 8);
            this.lvStatus.Name = "lvStatus";
            this.lvStatus.Size = new System.Drawing.Size(933, 346);
            this.lvStatus.TabIndex = 0;
            this.lvStatus.UseCompatibleStateImageBehavior = false;
            this.lvStatus.View = System.Windows.Forms.View.Details;
            this.lvStatus.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvStatus_ItemSelectionChanged);
            this.lvStatus.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvStatus_MouseDoubleClick);
            // 
            // chStatus
            // 
            this.chStatus.Text = "";
            this.chStatus.Width = 24;
            // 
            // chFile
            // 
            this.chFile.Text = "File";
            this.chFile.Width = 220;
            // 
            // chTrueFrameRate
            // 
            this.chTrueFrameRate.Text = "True Frame Rate";
            this.chTrueFrameRate.Width = 104;
            // 
            // chAverageFrameRate
            // 
            this.chAverageFrameRate.Text = "Avg Frame Rate";
            this.chAverageFrameRate.Width = 104;
            // 
            // chGuessFrameRate
            // 
            this.chGuessFrameRate.Text = "Guess Frame Rate";
            // 
            // chDetails
            // 
            this.chDetails.Text = "Warning/Error";
            this.chDetails.Width = 560;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(881, 369);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(60, 33);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCopyFrameRates
            // 
            this.btnCopyFrameRates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopyFrameRates.Enabled = false;
            this.btnCopyFrameRates.Location = new System.Drawing.Point(8, 369);
            this.btnCopyFrameRates.Name = "btnCopyFrameRates";
            this.btnCopyFrameRates.Size = new System.Drawing.Size(120, 33);
            this.btnCopyFrameRates.TabIndex = 3;
            this.btnCopyFrameRates.Text = "&Copy Frame Rates";
            this.btnCopyFrameRates.UseVisualStyleBackColor = true;
            this.btnCopyFrameRates.Click += new System.EventHandler(this.btnCopyFrameRates_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Location = new System.Drawing.Point(813, 369);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(60, 33);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "&Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnShowTimestamp
            // 
            this.btnShowTimestamp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShowTimestamp.Enabled = false;
            this.btnShowTimestamp.Location = new System.Drawing.Point(143, 369);
            this.btnShowTimestamp.Name = "btnShowTimestamp";
            this.btnShowTimestamp.Size = new System.Drawing.Size(146, 33);
            this.btnShowTimestamp.TabIndex = 4;
            this.btnShowTimestamp.Text = "Sho&w Timestamp";
            this.btnShowTimestamp.UseVisualStyleBackColor = true;
            this.btnShowTimestamp.Click += new System.EventHandler(this.btnShowTimestamp_Click);
            // 
            // frmStatus
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(949, 410);
            this.Controls.Add(this.btnShowTimestamp);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnCopyFrameRates);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lvStatus);
            this.Font = new System.Drawing.Font("Î¢ÈíÑÅºÚ", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(280, 180);
            this.Name = "frmStatus";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Status";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmStatus_FormClosing);
            this.Shown += new System.EventHandler(this.frmStatus_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvStatus;
        private System.Windows.Forms.ColumnHeader chFile;
        private System.Windows.Forms.ColumnHeader chStatus;
        private System.Windows.Forms.ColumnHeader chTrueFrameRate;
        private System.Windows.Forms.ColumnHeader chAverageFrameRate;
        private System.Windows.Forms.ColumnHeader chDetails;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCopyFrameRates;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnShowTimestamp;
        private System.Windows.Forms.ColumnHeader chGuessFrameRate;
    }
}