﻿
namespace FolderSyncForm
{
    partial class FolderSyncForm
    {
        private Label lbSource;
        private TextBox txtSource;
        private Button btnSource;
        private Label lbDest;
        private TextBox txtDest;
        private Button btnDest;

        private ComboBox cbType;
        private Button btnCompare;
        private Button btnOverwrite;
        private Button btnBackupList;
        private Button btnRestore;
        private DataGridView gv;
        private Button btnDeleteBackup;
        private ToolTip toolTip;
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
            components = new System.ComponentModel.Container();
            lbSource = new Label();
            txtSource = new TextBox();
            btnSource = new Button();
            lbDest = new Label();
            txtDest = new TextBox();
            btnDest = new Button();
            cbType = new ComboBox();
            btnCompare = new Button();
            btnOverwrite = new Button();
            btnBackupList = new Button();
            btnRestore = new Button();
            gv = new DataGridView();
            btnDeleteBackup = new Button();
            toolTip = new ToolTip(components);
            btnOpenSettings = new Button();
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
            lbSource.Size = new Size(131, 30);
            lbSource.TabIndex = 0;
            lbSource.Text = "來源（新資料）";
            lbSource.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtSource
            // 
            txtSource.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSource.Font = new Font("Segoe UI", 12F);
            txtSource.Location = new Point(147, 15);
            txtSource.Margin = new Padding(2);
            txtSource.Name = "txtSource";
            txtSource.PlaceholderText = "新檔案 的資料夾路徑";
            txtSource.Size = new Size(471, 29);
            txtSource.TabIndex = 1;
            // 
            // btnSource
            // 
            btnSource.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSource.BackColor = Color.SlateBlue;
            btnSource.Font = new Font("Segoe UI", 12F);
            btnSource.ForeColor = SystemColors.Control;
            btnSource.Location = new Point(624, 15);
            btnSource.Margin = new Padding(2);
            btnSource.Name = "btnSource";
            btnSource.Size = new Size(110, 30);
            btnSource.TabIndex = 2;
            btnSource.Text = "選擇來源";
            toolTip.SetToolTip(btnSource, "選擇 新檔案 資料夾");
            btnSource.UseVisualStyleBackColor = false;
            btnSource.Click += btnSource_Click;
            // 
            // lbDest
            // 
            lbDest.Font = new Font("Segoe UI", 12F);
            lbDest.Location = new Point(11, 52);
            lbDest.Margin = new Padding(2, 0, 2, 0);
            lbDest.Name = "lbDest";
            lbDest.Size = new Size(132, 30);
            lbDest.TabIndex = 3;
            lbDest.Text = "目標（被更新）";
            lbDest.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDest
            // 
            txtDest.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDest.Font = new Font("Segoe UI", 12F);
            txtDest.Location = new Point(147, 52);
            txtDest.Margin = new Padding(2);
            txtDest.Name = "txtDest";
            txtDest.PlaceholderText = "被覆蓋 的資料夾路徑";
            txtDest.Size = new Size(471, 29);
            txtDest.TabIndex = 4;
            // 
            // btnDest
            // 
            btnDest.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDest.BackColor = Color.RoyalBlue;
            btnDest.Font = new Font("Segoe UI", 12F);
            btnDest.ForeColor = SystemColors.Control;
            btnDest.Location = new Point(624, 52);
            btnDest.Margin = new Padding(2);
            btnDest.Name = "btnDest";
            btnDest.Size = new Size(110, 30);
            btnDest.TabIndex = 5;
            btnDest.Text = "選擇目標";
            toolTip.SetToolTip(btnDest, "選擇 被覆蓋 資料夾");
            btnDest.UseVisualStyleBackColor = false;
            btnDest.Click += btnDest_Click;
            // 
            // cbType
            // 
            cbType.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbType.Font = new Font("Segoe UI", 12F);
            cbType.FormattingEnabled = true;
            cbType.Location = new Point(624, 124);
            cbType.Margin = new Padding(2);
            cbType.Name = "cbType";
            cbType.Size = new Size(110, 29);
            cbType.TabIndex = 6;
            toolTip.SetToolTip(cbType, ".NET站台模式 先關閉站台再操作資料夾\r\n資料夾模式 直接操作資料夾");
            // 
            // btnCompare
            // 
            btnCompare.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCompare.BackColor = Color.LightGreen;
            btnCompare.Font = new Font("Segoe UI", 12F);
            btnCompare.Location = new Point(624, 157);
            btnCompare.Margin = new Padding(2);
            btnCompare.Name = "btnCompare";
            btnCompare.Size = new Size(110, 30);
            btnCompare.TabIndex = 7;
            btnCompare.Text = "比對";
            toolTip.SetToolTip(btnCompare, "比較 檔案 差異");
            btnCompare.UseVisualStyleBackColor = false;
            btnCompare.Click += btnCompare_Click;
            // 
            // btnOverwrite
            // 
            btnOverwrite.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOverwrite.BackColor = Color.LightCoral;
            btnOverwrite.Font = new Font("Segoe UI", 12F);
            btnOverwrite.Location = new Point(624, 191);
            btnOverwrite.Margin = new Padding(2);
            btnOverwrite.Name = "btnOverwrite";
            btnOverwrite.Size = new Size(110, 30);
            btnOverwrite.TabIndex = 9;
            btnOverwrite.Text = "更新";
            toolTip.SetToolTip(btnOverwrite, "備份後覆蓋資料夾");
            btnOverwrite.UseVisualStyleBackColor = false;
            btnOverwrite.Click += btnOverwrite_Click;
            // 
            // btnBackupList
            // 
            btnBackupList.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnBackupList.BackColor = Color.LightGreen;
            btnBackupList.Font = new Font("Segoe UI", 12F);
            btnBackupList.Location = new Point(624, 328);
            btnBackupList.Margin = new Padding(2);
            btnBackupList.Name = "btnBackupList";
            btnBackupList.Size = new Size(110, 30);
            btnBackupList.TabIndex = 10;
            btnBackupList.Text = "備份紀錄";
            toolTip.SetToolTip(btnBackupList, "顯示 備份清單");
            btnBackupList.UseVisualStyleBackColor = false;
            btnBackupList.Click += btnBackupList_Click;
            // 
            // btnRestore
            // 
            btnRestore.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnRestore.BackColor = Color.LightCoral;
            btnRestore.Font = new Font("Segoe UI", 12F);
            btnRestore.Location = new Point(624, 362);
            btnRestore.Margin = new Padding(2);
            btnRestore.Name = "btnRestore";
            btnRestore.Size = new Size(110, 30);
            btnRestore.TabIndex = 12;
            btnRestore.Text = "還原備份";
            toolTip.SetToolTip(btnRestore, "從 備份清單 還原檔案");
            btnRestore.UseVisualStyleBackColor = false;
            btnRestore.Click += btnRestore_Click;
            // 
            // gv
            // 
            gv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gv.Location = new Point(12, 90);
            gv.Name = "gv";
            gv.Size = new Size(606, 336);
            gv.TabIndex = 13;
            // 
            // btnDeleteBackup
            // 
            btnDeleteBackup.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDeleteBackup.BackColor = Color.Thistle;
            btnDeleteBackup.Font = new Font("Segoe UI", 12F);
            btnDeleteBackup.Location = new Point(624, 396);
            btnDeleteBackup.Margin = new Padding(2);
            btnDeleteBackup.Name = "btnDeleteBackup";
            btnDeleteBackup.Size = new Size(110, 30);
            btnDeleteBackup.TabIndex = 14;
            btnDeleteBackup.Text = "刪除備份";
            toolTip.SetToolTip(btnDeleteBackup, "刪除 選擇的備份紀錄");
            btnDeleteBackup.UseVisualStyleBackColor = false;
            btnDeleteBackup.Click += btnDeleteBackup_Click;
            // 
            // toolTip
            // 
            toolTip.AutomaticDelay = 0;
            toolTip.AutoPopDelay = 0;
            toolTip.InitialDelay = 0;
            toolTip.IsBalloon = true;
            toolTip.ReshowDelay = 0;
            toolTip.ShowAlways = true;
            toolTip.ToolTipIcon = ToolTipIcon.Info;
            toolTip.ToolTipTitle = "說明";
            // 
            // btnOpenSettings
            // 
            btnOpenSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenSettings.BackColor = Color.LightGreen;
            btnOpenSettings.Font = new Font("Segoe UI", 12F);
            btnOpenSettings.Location = new Point(624, 90);
            btnOpenSettings.Margin = new Padding(2);
            btnOpenSettings.Name = "btnOpenSettings";
            btnOpenSettings.Size = new Size(110, 30);
            btnOpenSettings.TabIndex = 15;
            btnOpenSettings.Text = "設定檔";
            toolTip.SetToolTip(btnOpenSettings, "比較 檔案 差異");
            btnOpenSettings.UseVisualStyleBackColor = false;
            btnOpenSettings.Click += btnOpenSettings_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(740, 440);
            Controls.Add(btnOpenSettings);
            Controls.Add(btnDeleteBackup);
            Controls.Add(lbSource);
            Controls.Add(txtSource);
            Controls.Add(btnSource);
            Controls.Add(lbDest);
            Controls.Add(txtDest);
            Controls.Add(btnDest);
            Controls.Add(cbType);
            Controls.Add(btnCompare);
            Controls.Add(btnOverwrite);
            Controls.Add(btnBackupList);
            Controls.Add(btnRestore);
            Controls.Add(gv);
            Margin = new Padding(2);
            Name = "MainForm";
            Text = "資料夾比對";
            ((System.ComponentModel.ISupportInitialize)gv).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion




        private Button btnOpenSettings;
    }
}
