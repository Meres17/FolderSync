using FolderSyncForm.Helpers;

namespace FolderSyncForm.Forms
{
    /// <summary>
    /// 帶可捲動清單的確認視窗，用於清單可能很長的最終確認。
    /// </summary>
    public partial class ConfirmListForm : Form
    {
        public ConfirmListForm(string message, IEnumerable<string> lines)
        {
            InitializeComponent();
            lblMessage.Text = message;
            txtList.Lines = lines.ToArray();
            FormTheme.Apply(this);
        }
    }
}
