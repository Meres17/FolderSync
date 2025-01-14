namespace FolderSyncForm
{
    public partial class DialogForm : Form
    {
        public DialogForm(string message, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            InitializeComponent();
            lblMessage.Text = message;
            btnCancel.Visible = buttons == MessageBoxButtons.OKCancel;
        }

        protected void BtnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
