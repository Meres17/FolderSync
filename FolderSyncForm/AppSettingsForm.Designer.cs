namespace FolderSyncForm
{
    partial class AppSettingsForm
    {
        private Label lbSource;
        private TextBox txtSource;
        private Button btnSource;

        private Label lbDest;
        private TextBox txtDest;
        private Button btnDest;
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
            lbSource = new Label();
            txtSource = new TextBox();
            btnSource = new Button();
            lbDest = new Label();
            txtDest = new TextBox();
            btnDest = new Button();
            lbtxtIgnoreFolders = new Label();
            txtIgnoreFolders = new TextBox();
            txtIgnoreFiles = new TextBox();
            lbtxtIgnoreFiles = new Label();
            btnSave = new Button();
            SuspendLayout();
            // 
            // lbSource
            // 
            lbSource.Font = new Font("Segoe UI", 12F);
            lbSource.ForeColor = Color.FromArgb(51, 51, 51);
            lbSource.Location = new Point(11, 9);
            lbSource.Margin = new Padding(2, 0, 2, 0);
            lbSource.Name = "lbSource";
            lbSource.Size = new Size(163, 30);
            lbSource.TabIndex = 0;
            lbSource.Text = "預設來源（新資料）";
            lbSource.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtSource
            // 
            txtSource.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSource.Font = new Font("Segoe UI", 12F);
            txtSource.Location = new Point(178, 11);
            txtSource.Margin = new Padding(2);
            txtSource.Name = "txtSource";
            txtSource.PlaceholderText = "新檔案 的資料夾路徑";
            txtSource.Size = new Size(567, 29);
            txtSource.TabIndex = 1;
            // 
            // btnSource
            // 
            btnSource.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSource.BackColor = Color.SlateBlue;
            btnSource.Font = new Font("Segoe UI", 12F);
            btnSource.ForeColor = SystemColors.Control;
            btnSource.Location = new Point(749, 11);
            btnSource.Margin = new Padding(2);
            btnSource.Name = "btnSource";
            btnSource.Size = new Size(110, 30);
            btnSource.TabIndex = 2;
            btnSource.Text = "選擇來源";
            btnSource.UseVisualStyleBackColor = false;
            btnSource.Click += btnSource_Click;
            // 
            // lbDest
            // 
            lbDest.Font = new Font("Segoe UI", 12F);
            lbDest.Location = new Point(11, 50);
            lbDest.Margin = new Padding(2, 0, 2, 0);
            lbDest.Name = "lbDest";
            lbDest.Size = new Size(163, 30);
            lbDest.TabIndex = 3;
            lbDest.Text = "預設目標（被更新）";
            lbDest.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDest
            // 
            txtDest.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDest.Font = new Font("Segoe UI", 12F);
            txtDest.Location = new Point(178, 52);
            txtDest.Margin = new Padding(2);
            txtDest.Name = "txtDest";
            txtDest.PlaceholderText = "被覆蓋 的資料夾路徑";
            txtDest.Size = new Size(567, 29);
            txtDest.TabIndex = 4;
            // 
            // btnDest
            // 
            btnDest.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDest.BackColor = Color.RoyalBlue;
            btnDest.Font = new Font("Segoe UI", 12F);
            btnDest.ForeColor = SystemColors.Control;
            btnDest.Location = new Point(749, 52);
            btnDest.Margin = new Padding(2);
            btnDest.Name = "btnDest";
            btnDest.Size = new Size(110, 30);
            btnDest.TabIndex = 5;
            btnDest.Text = "選擇目標";
            btnDest.UseVisualStyleBackColor = false;
            btnDest.Click += btnDest_Click;
            // 
            // lbtxtIgnoreFolders
            // 
            lbtxtIgnoreFolders.Font = new Font("Segoe UI", 12F);
            lbtxtIgnoreFolders.ForeColor = Color.FromArgb(51, 51, 51);
            lbtxtIgnoreFolders.Location = new Point(11, 96);
            lbtxtIgnoreFolders.Margin = new Padding(2, 0, 2, 0);
            lbtxtIgnoreFolders.Name = "lbtxtIgnoreFolders";
            lbtxtIgnoreFolders.Size = new Size(163, 30);
            lbtxtIgnoreFolders.TabIndex = 6;
            lbtxtIgnoreFolders.Text = "忽略資料夾路徑";
            lbtxtIgnoreFolders.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtIgnoreFolders
            // 
            txtIgnoreFolders.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtIgnoreFolders.Font = new Font("Segoe UI", 12F);
            txtIgnoreFolders.Location = new Point(178, 96);
            txtIgnoreFolders.Margin = new Padding(2);
            txtIgnoreFolders.Multiline = true;
            txtIgnoreFolders.Name = "txtIgnoreFolders";
            txtIgnoreFolders.PlaceholderText = "忽略的資料夾路徑";
            txtIgnoreFolders.Size = new Size(567, 147);
            txtIgnoreFolders.TabIndex = 7;
            // 
            // txtIgnoreFiles
            // 
            txtIgnoreFiles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtIgnoreFiles.Font = new Font("Segoe UI", 12F);
            txtIgnoreFiles.Location = new Point(178, 249);
            txtIgnoreFiles.Margin = new Padding(2);
            txtIgnoreFiles.Multiline = true;
            txtIgnoreFiles.Name = "txtIgnoreFiles";
            txtIgnoreFiles.PlaceholderText = "忽略的檔案名稱";
            txtIgnoreFiles.Size = new Size(567, 147);
            txtIgnoreFiles.TabIndex = 8;
            // 
            // lbtxtIgnoreFiles
            // 
            lbtxtIgnoreFiles.Font = new Font("Segoe UI", 12F);
            lbtxtIgnoreFiles.ForeColor = Color.FromArgb(51, 51, 51);
            lbtxtIgnoreFiles.Location = new Point(11, 247);
            lbtxtIgnoreFiles.Margin = new Padding(2, 0, 2, 0);
            lbtxtIgnoreFiles.Name = "lbtxtIgnoreFiles";
            lbtxtIgnoreFiles.Size = new Size(163, 30);
            lbtxtIgnoreFiles.TabIndex = 9;
            lbtxtIgnoreFiles.Text = "忽略檔案名稱";
            lbtxtIgnoreFiles.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSave.BackColor = Color.RoyalBlue;
            btnSave.Font = new Font("Segoe UI", 12F);
            btnSave.ForeColor = SystemColors.Control;
            btnSave.Location = new Point(755, 366);
            btnSave.Margin = new Padding(2);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(110, 30);
            btnSave.TabIndex = 10;
            btnSave.Text = "保存";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // AppSettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(876, 450);
            Controls.Add(btnSave);
            Controls.Add(lbtxtIgnoreFiles);
            Controls.Add(txtIgnoreFiles);
            Controls.Add(txtIgnoreFolders);
            Controls.Add(lbtxtIgnoreFolders);
            Controls.Add(lbSource);
            Controls.Add(txtSource);
            Controls.Add(btnSource);
            Controls.Add(lbDest);
            Controls.Add(txtDest);
            Controls.Add(btnDest);
            Name = "AppSettingsForm";
            Text = "設定檔調整";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbtxtIgnoreFolders;
        private TextBox txtIgnoreFolders;
        private TextBox txtIgnoreFiles;
        private Label lbtxtIgnoreFiles;
        private Button btnSave;
    }
}