namespace FolderSyncForm
{
    partial class MainForm
    {
        private Label lbSource;
        private Label lbTarget;
        private TextBox txtSource;
        private TextBox txtTarget;
        private Button btnSource;
        private Button btnTarget;
        private Button btnCompare;
        private Button btnCopy;
        private Button btnBackupList;
        private Button btnRestore;
        private DataGridView gvDiff;

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
            lbTarget = new Label();
            txtSource = new TextBox();
            txtTarget = new TextBox();
            btnSource = new Button();
            btnTarget = new Button();
            btnCompare = new Button();
            btnCopy = new Button();
            btnBackupList = new Button();
            btnRestore = new Button();
            gvDiff = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)gvDiff).BeginInit();
            SuspendLayout();
            // 
            // _lbSource
            // 
            lbSource.Font = new Font("Segoe UI", 12F);
            lbSource.ForeColor = Color.FromArgb(51, 51, 51);
            lbSource.Location = new Point(12, 15);
            lbSource.Margin = new Padding(2, 0, 2, 0);
            lbSource.Name = "_lbSource";
            lbSource.Size = new Size(117, 30);
            lbSource.TabIndex = 0;
            lbSource.Text = "來源(新資料)";
            lbSource.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _lbTarget
            // 
            lbTarget.Font = new Font("Segoe UI", 12F);
            lbTarget.Location = new Point(11, 52);
            lbTarget.Margin = new Padding(2, 0, 2, 0);
            lbTarget.Name = "_lbTarget";
            lbTarget.Size = new Size(117, 30);
            lbTarget.TabIndex = 1;
            lbTarget.Text = "目標(被更新)";
            lbTarget.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _txtSource
            // 
            txtSource.Font = new Font("Segoe UI", 12F);
            txtSource.Location = new Point(132, 15);
            txtSource.Margin = new Padding(2);
            txtSource.Name = "_txtSource";
            txtSource.Size = new Size(474, 29);
            txtSource.TabIndex = 2;
            // 
            // _txtTarget
            // 
            txtTarget.Font = new Font("Segoe UI", 12F);
            txtTarget.Location = new Point(132, 52);
            txtTarget.Margin = new Padding(2);
            txtTarget.Name = "_txtTarget";
            txtTarget.Size = new Size(474, 29);
            txtTarget.TabIndex = 3;
            // 
            // _btnSource
            // 
            btnSource.Font = new Font("Segoe UI", 12F);
            btnSource.Location = new Point(613, 15);
            btnSource.Margin = new Padding(2);
            btnSource.Name = "_btnSource";
            btnSource.Size = new Size(93, 30);
            btnSource.TabIndex = 4;
            btnSource.Text = "選擇來源";
            btnSource.Click += btnSource_Click;
            // 
            // _btnTarget
            // 
            btnTarget.Font = new Font("Segoe UI", 12F);
            btnTarget.Location = new Point(613, 52);
            btnTarget.Margin = new Padding(2);
            btnTarget.Name = "_btnTarget";
            btnTarget.Size = new Size(93, 30);
            btnTarget.TabIndex = 5;
            btnTarget.Text = "選擇目標";
            btnTarget.Click += btnTarget_Click;
            // 
            // _btnCompare
            // 
            btnCompare.Font = new Font("Segoe UI", 12F);
            btnCompare.Location = new Point(613, 90);
            btnCompare.Margin = new Padding(2);
            btnCompare.Name = "_btnCompare";
            btnCompare.Size = new Size(93, 30);
            btnCompare.TabIndex = 6;
            btnCompare.Text = "比對";
            btnCompare.Click += btnCompare_Click;
            // 
            // _btnCopy
            // 
            btnCopy.Font = new Font("Segoe UI", 12F);
            btnCopy.Location = new Point(613, 128);
            btnCopy.Margin = new Padding(2);
            btnCopy.Name = "_btnCopy";
            btnCopy.Size = new Size(93, 30);
            btnCopy.TabIndex = 7;
            btnCopy.Text = "複製";
            btnCopy.Click += btnCopy_Click;
            // 
            // _btnBackupList
            // 
            btnBackupList.Font = new Font("Segoe UI", 12F);
            btnBackupList.Location = new Point(613, 165);
            btnBackupList.Margin = new Padding(2);
            btnBackupList.Name = "_btnBackupList";
            btnBackupList.Size = new Size(93, 30);
            btnBackupList.TabIndex = 8;
            btnBackupList.Text = "備份紀錄";
            btnBackupList.Click += btnBackupList_Click;
            // 
            // _btnRestore
            // 
            btnRestore.Font = new Font("Segoe UI", 12F);
            btnRestore.Location = new Point(613, 202);
            btnRestore.Margin = new Padding(2);
            btnRestore.Name = "_btnRestore";
            btnRestore.Size = new Size(93, 30);
            btnRestore.TabIndex = 9;
            btnRestore.Text = "還原";
            btnRestore.Click += btnRestore_Click;
            // 
            // _gvDiff
            // 
            gvDiff.Location = new Point(12, 90);
            gvDiff.Name = "_gvDiff";
            gvDiff.Size = new Size(594, 336);
            gvDiff.TabIndex = 10;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(722, 450);
            Controls.Add(btnRestore);
            Controls.Add(btnBackupList);
            Controls.Add(btnCopy);
            Controls.Add(btnCompare);
            Controls.Add(btnTarget);
            Controls.Add(btnSource);
            Controls.Add(txtTarget);
            Controls.Add(txtSource);
            Controls.Add(lbTarget);
            Controls.Add(lbSource);
            Controls.Add(gvDiff);
            Margin = new Padding(2);
            Name = "MainForm";
            Text = "資料夾比對";
            ((System.ComponentModel.ISupportInitialize)gvDiff).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion



    }
}
