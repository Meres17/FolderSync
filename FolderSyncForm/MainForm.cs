using FolderSyncCore;

using FolderSyncForm.Extensions;

namespace FolderSyncForm
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            txtSource.BindFolderDialog();
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            txtTarget.BindFolderDialog();
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            gvDiff.DataSource = FolderComparer.Diff(txtSource.Text, txtTarget.Text);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var dtos = (List<FileDTO>)gvDiff.DataSource;
            if (dtos == null) return;
            FolderComparer.Backup(dtos, txtSource.Text, txtTarget.Text);
            gvDiff.DataSource = FolderComparer.Diff(txtSource.Text, txtTarget.Text);
        }

        private void btnBackupList_Click(object sender, EventArgs e)
        {
            gvDiff.DataSource = FolderComparer.BackupFolders(txtSource.Text);
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (gvDiff.SelectedRows.Count == 0)
            {
                MessageBox.Show("請選擇要還原的檔案");
                return;
            }

            if (gvDiff.SelectedRows.Count > 1)
            {
                MessageBox.Show("要還原的檔案只能選一個");
                return;
            }

            // 取得選取的 FolderDTO 物件
            var dto = (FolderDTO)gvDiff.SelectedRows[0].DataBoundItem;

            FolderComparer.Restore(dto.完整路徑, txtTarget.Text);
            MessageBox.Show("還原成功");
        }
    }
}
