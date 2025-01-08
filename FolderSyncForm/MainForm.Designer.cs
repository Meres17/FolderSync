namespace FolderSyncForm
{
    partial class MainForm
    {
        private Label lbSource;
        private Label lbDest;
        private TextBox txtSource;
        private TextBox txtDest;
        private Button btnSource;
        private Button btnDest;
        private Button btnCompare;
        private Button btnCopy;
        private Button btnBackupList;
        private Button btnRestore;
        private DataGridView gv;

        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lbSource = new Label();
            lbDest = new Label();
            txtSource = new TextBox();
            txtDest = new TextBox();
            btnSource = new Button();
            btnDest = new Button();
            btnCompare = new Button();
            btnCopy = new Button();
            btnBackupList = new Button();
            btnRestore = new Button();
            gv = new DataGridView();
            btnUpdateSite = new Button();
            btnRestoreSite = new Button();
            ((System.ComponentModel.ISupportInitialize)gv).BeginInit();
            SuspendLayout();
            // 
            // lbSource
            // 
            lbSource.Font = new Font("Segoe UI", 12F);
            lbSource.ForeColor = Color.FromArgb(51, 51, 51);
            lbSource.Location = new Point(12, 15);
            lbSource.Margin = new Padding(2, 0, 2, 0);
            lbSource.Name = "lbSource";
            lbSource.Size = new Size(117, 30);
            lbSource.TabIndex = 0;
            lbSource.Text = "來源(新資料)";
            lbSource.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbDest
            // 
            lbDest.Font = new Font("Segoe UI", 12F);
            lbDest.Location = new Point(11, 52);
            lbDest.Margin = new Padding(2, 0, 2, 0);
            lbDest.Name = "lbDest";
            lbDest.Size = new Size(117, 30);
            lbDest.TabIndex = 1;
            lbDest.Text = "目標(被更新)";
            lbDest.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtSource
            // 
            txtSource.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSource.Font = new Font("Segoe UI", 12F);
            txtSource.Location = new Point(132, 15);
            txtSource.Margin = new Padding(2);
            txtSource.Name = "txtSource";
            txtSource.Size = new Size(474, 29);
            txtSource.TabIndex = 2;
            // 
            // txtDest
            // 
            txtDest.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDest.Font = new Font("Segoe UI", 12F);
            txtDest.Location = new Point(132, 52);
            txtDest.Margin = new Padding(2);
            txtDest.Name = "txtDest";
            txtDest.Size = new Size(474, 29);
            txtDest.TabIndex = 3;
            // 
            // btnSource
            // 
            btnSource.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSource.Font = new Font("Segoe UI", 12F);
            btnSource.Location = new Point(613, 15);
            btnSource.Margin = new Padding(2);
            btnSource.Name = "btnSource";
            btnSource.Size = new Size(93, 30);
            btnSource.TabIndex = 4;
            btnSource.Text = "選擇來源";
            btnSource.Click += btnSource_Click;
            // 
            // btnDest
            // 
            btnDest.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDest.Font = new Font("Segoe UI", 12F);
            btnDest.Location = new Point(613, 52);
            btnDest.Margin = new Padding(2);
            btnDest.Name = "btnDest";
            btnDest.Size = new Size(93, 30);
            btnDest.TabIndex = 5;
            btnDest.Text = "選擇目標";
            btnDest.Click += btnDest_Click;
            // 
            // btnCompare
            // 
            btnCompare.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCompare.Font = new Font("Segoe UI", 12F);
            btnCompare.Location = new Point(613, 90);
            btnCompare.Margin = new Padding(2);
            btnCompare.Name = "btnCompare";
            btnCompare.Size = new Size(93, 30);
            btnCompare.TabIndex = 6;
            btnCompare.Text = "比對";
            btnCompare.Click += btnCompare_Click;
            // 
            // btnCopy
            // 
            btnCopy.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCopy.Font = new Font("Segoe UI", 12F);
            btnCopy.Location = new Point(613, 158);
            btnCopy.Margin = new Padding(2);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(93, 30);
            btnCopy.TabIndex = 7;
            btnCopy.Text = "複製";
            btnCopy.Click += btnCopy_Click;
            // 
            // btnBackupList
            // 
            btnBackupList.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnBackupList.Font = new Font("Segoe UI", 12F);
            btnBackupList.Location = new Point(613, 328);
            btnBackupList.Margin = new Padding(2);
            btnBackupList.Name = "btnBackupList";
            btnBackupList.Size = new Size(93, 30);
            btnBackupList.TabIndex = 8;
            btnBackupList.Text = "備份紀錄";
            btnBackupList.Click += btnBackupList_Click;
            // 
            // btnRestore
            // 
            btnRestore.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnRestore.Font = new Font("Segoe UI", 12F);
            btnRestore.Location = new Point(613, 396);
            btnRestore.Margin = new Padding(2);
            btnRestore.Name = "btnRestore";
            btnRestore.Size = new Size(93, 30);
            btnRestore.TabIndex = 9;
            btnRestore.Text = "還原";
            btnRestore.Click += btnRestore_Click;
            // 
            // gvDiff
            // 
            gv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gv.Location = new Point(12, 90);
            gv.Name = "gv";
            gv.Size = new Size(594, 336);
            gv.TabIndex = 10;
            // 
            // btnUpdateSite
            // 
            btnUpdateSite.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUpdateSite.Font = new Font("Segoe UI", 12F);
            btnUpdateSite.Location = new Point(613, 124);
            btnUpdateSite.Margin = new Padding(2);
            btnUpdateSite.Name = "btnUpdateSite";
            btnUpdateSite.Size = new Size(93, 30);
            btnUpdateSite.TabIndex = 11;
            btnUpdateSite.Text = "更新站台";
            btnUpdateSite.Click += btnUpdateSite_Click;
            // 
            // btnRestoreSite
            // 
            btnRestoreSite.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnRestoreSite.Font = new Font("Segoe UI", 12F);
            btnRestoreSite.Location = new Point(613, 362);
            btnRestoreSite.Margin = new Padding(2);
            btnRestoreSite.Name = "btnRestoreSite";
            btnRestoreSite.Size = new Size(93, 30);
            btnRestoreSite.TabIndex = 12;
            btnRestoreSite.Text = "還原站台";
            btnRestoreSite.Click += btnRestoreSite_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(722, 450);
            Controls.Add(btnRestoreSite);
            Controls.Add(btnUpdateSite);
            Controls.Add(btnRestore);
            Controls.Add(btnBackupList);
            Controls.Add(btnCopy);
            Controls.Add(btnCompare);
            Controls.Add(btnDest);
            Controls.Add(btnSource);
            Controls.Add(txtDest);
            Controls.Add(txtSource);
            Controls.Add(lbDest);
            Controls.Add(lbSource);
            Controls.Add(gv);
            Margin = new Padding(2);
            Name = "MainForm";
            Text = "資料夾比對";
            ((System.ComponentModel.ISupportInitialize)gv).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion



        private Button btnUpdateSite;
        private Button btnRestoreSite;
    }
}
