using FolderSyncCore;

using FolderSyncForm.Extensions;

namespace FolderSyncForm
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        public MainForm()
        {
            InitializeComponent();
            var settings = AppSettings.Build();
            txtSource.Text = settings.Source;
            txtTarget.Text = settings.Target;
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
            MessageBox.Show("複製成功，差異檔案已備份至本目錄底下backup/來源_目標/時間標記");
        }

        private void btnUpdateSite_Click(object sender, EventArgs e)
        {
            var comparer = new FolderComparer(txtSource.Text, txtTarget.Text);

            if (comparer.ExistOffline())
            {
                comparer.CloseSite();
                comparer.Backup();
                comparer.OpenSite();
                MessageBox.Show("站台更新成功，差異檔案已備份至本目錄底下backup/來源_目標/時間標記");
            }
            else
            {
                MessageBox.Show("找不到App_offline.htm，請將檔案放置本執行檔旁邊");
            }
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
