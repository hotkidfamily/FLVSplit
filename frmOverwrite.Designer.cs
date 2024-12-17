namespace JDP {
    partial class frmOverwrite {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOverwrite));
            this.lblMessage1 = new System.Windows.Forms.Label();
            this.lblFileName = new System.Windows.Forms.Label();
            this.lblMessage2 = new System.Windows.Forms.Label();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnYesAll = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.btnNoAll = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblMessage1
            // 
            this.lblMessage1.Location = new System.Drawing.Point(8, 8);
            this.lblMessage1.Name = "lblMessage1";
            this.lblMessage1.Size = new System.Drawing.Size(342, 24);
            this.lblMessage1.TabIndex = 0;
            this.lblMessage1.Text = "The following file already exists:";
            // 
            // lblFileName
            // 
            this.lblFileName.Location = new System.Drawing.Point(8, 32);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(336, 13);
            this.lblFileName.TabIndex = 1;
            // 
            // lblMessage2
            // 
            this.lblMessage2.Location = new System.Drawing.Point(8, 56);
            this.lblMessage2.Name = "lblMessage2";
            this.lblMessage2.Size = new System.Drawing.Size(342, 57);
            this.lblMessage2.TabIndex = 2;
            this.lblMessage2.Text = "Do you want to overwrite it?";
            // 
            // btnYes
            // 
            this.btnYes.Location = new System.Drawing.Point(8, 127);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(65, 29);
            this.btnYes.TabIndex = 3;
            this.btnYes.Text = "&Yes";
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // btnYesAll
            // 
            this.btnYesAll.Location = new System.Drawing.Point(76, 127);
            this.btnYesAll.Name = "btnYesAll";
            this.btnYesAll.Size = new System.Drawing.Size(65, 29);
            this.btnYesAll.TabIndex = 4;
            this.btnYesAll.Text = "Y&es to All";
            this.btnYesAll.UseVisualStyleBackColor = true;
            this.btnYesAll.Click += new System.EventHandler(this.btnYesAll_Click);
            // 
            // btnNo
            // 
            this.btnNo.Location = new System.Drawing.Point(144, 127);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(65, 29);
            this.btnNo.TabIndex = 5;
            this.btnNo.Text = "&No";
            this.btnNo.UseVisualStyleBackColor = true;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // btnNoAll
            // 
            this.btnNoAll.Location = new System.Drawing.Point(212, 127);
            this.btnNoAll.Name = "btnNoAll";
            this.btnNoAll.Size = new System.Drawing.Size(65, 29);
            this.btnNoAll.TabIndex = 6;
            this.btnNoAll.Text = "N&o to All";
            this.btnNoAll.UseVisualStyleBackColor = true;
            this.btnNoAll.Click += new System.EventHandler(this.btnNoAll_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(280, 127);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(65, 29);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmOverwrite
            // 
            this.AcceptButton = this.btnYes;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(513, 201);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnNoAll);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYesAll);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.lblMessage2);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.lblMessage1);
            this.Font = new System.Drawing.Font("Î¢ÈíÑÅºÚ", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOverwrite";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Overwrite?";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMessage1;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label lblMessage2;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnYesAll;
        private System.Windows.Forms.Button btnNo;
        private System.Windows.Forms.Button btnNoAll;
        private System.Windows.Forms.Button btnCancel;
    }
}