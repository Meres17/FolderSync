namespace FolderSyncForm.Forms
{
    partial class ConfirmListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Label lblMessage;
        private TextBox txtList;
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
            txtList = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            SuspendLayout();
            //
            // lblMessage
            //
            lblMessage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblMessage.Location = new Point(12, 9);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(676, 56);
            lblMessage.TabIndex = 0;
            lblMessage.Text = "訊息";
            //
            // txtList
            //
            txtList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtList.Location = new Point(12, 68);
            txtList.Multiline = true;
            txtList.Name = "txtList";
            txtList.ReadOnly = true;
            txtList.ScrollBars = ScrollBars.Both;
            txtList.Size = new Size(676, 340);
            txtList.TabIndex = 1;
            txtList.WordWrap = false;
            //
            // btnOK
            //
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(468, 418);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(107, 34);
            btnOK.TabIndex = 2;
            btnOK.Text = "確定更新";
            //
            // btnCancel
            //
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(581, 418);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(107, 34);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "取消";
            //
            // ConfirmListForm
            //
            CancelButton = btnCancel;
            ClientSize = new Size(700, 464);
            Controls.Add(lblMessage);
            Controls.Add(txtList);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(480, 320);
            Name = "ConfirmListForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "確認";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
