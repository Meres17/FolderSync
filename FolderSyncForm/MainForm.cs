using FolderSyncCore;

using FolderSyncForm.Extensions;

namespace FolderSyncForm
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        public MainForm()
        {
            InitializeComponent();
            //txtSource.Text = "D:\\海巡\\Bk\\20241224.2";
            //txtTarget.Text = "D:\\海巡\\Bk\\20241223.3";
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
            gvDiff.DataSource = new FolderComparer(txtSource.Text, txtTarget.Text).GetDiffFiles();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var comparer = new FolderComparer(txtSource.Text, txtTarget.Text);
            comparer.Backup();
            gvDiff.DataSource = comparer.GetDiffFiles();
        }

        private void btnBackupList_Click(object sender, EventArgs e)
        {
            gvDiff.DataSource = new FolderComparer(txtSource.Text, txtTarget.Text).GetBackupFolders();
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

            new FolderComparer(txtSource.Text, txtTarget.Text).Restore(dto.完整路徑);

            MessageBox.Show("還原成功");
        }
    }
}
