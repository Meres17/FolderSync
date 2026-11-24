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
        private Button btnConfirmOverwrite;
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
            btnConfirmOverwrite = new Button();
            btnCancel = new Button();
            SuspendLayout();
            //
            // lblMessage
            //
            lblMessage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblMessage.Location = new Point(12, 9);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(676, 108);
            lblMessage.TabIndex = 0;
            lblMessage.Text = "訊息";
            //
            // txtList
            //
            txtList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtList.Location = new Point(12, 120);
            txtList.Multiline = true;
            txtList.Name = "txtList";
            txtList.ReadOnly = true;
            txtList.ScrollBars = ScrollBars.Both;
            txtList.Size = new Size(676, 288);
            txtList.TabIndex = 1;
            txtList.WordWrap = false;
            //
            // btnConfirmOverwrite
            //
            btnConfirmOverwrite.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnConfirmOverwrite.DialogResult = DialogResult.OK;
            btnConfirmOverwrite.Location = new Point(468, 418);
            btnConfirmOverwrite.Name = "btnConfirmOverwrite";
            btnConfirmOverwrite.Size = new Size(107, 34);
            btnConfirmOverwrite.TabIndex = 2;
            btnConfirmOverwrite.Text = "確定更新";
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
            Controls.Add(btnConfirmOverwrite);
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
