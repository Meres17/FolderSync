namespace FolderSyncForm
{
    partial class DialogForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Label lblMessage;
        private Button btnOK;
        private Button btnCancel;

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
            lblMessage = new Label();
            btnOK = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblMessage
            // 
            lblMessage.Width = 500;
            lblMessage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblMessage.Font = new Font("微軟正黑體", 12F);
            lblMessage.Location = new Point(12, 9);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(213, 86);
            lblMessage.TabIndex = 0;
            lblMessage.Text = "訊息";
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnOK.BackColor = Color.LightBlue;
            btnOK.Font = new Font("微軟正黑體", 12F);
            btnOK.Location = new Point(60, 98);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(107, 30);
            btnOK.TabIndex = 1;
            btnOK.Text = "確定";
            btnOK.UseVisualStyleBackColor = false;
            btnOK.Click += BtnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnCancel.BackColor = Color.LightCoral;
            btnCancel.Font = new Font("微軟正黑體", 12F);
            btnCancel.Location = new Point(60, 134);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(107, 30);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Visible = false;
            btnCancel.Click += BtnCancel_Click;
            // 
            // DialogForm
            // 
            ClientSize = new Size(237, 176);
            ControlBox = false;
            Controls.Add(lblMessage);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Name = "DialogForm";
            Text = "訊息";
            ResumeLayout(false);
        }



        #endregion
    }
}