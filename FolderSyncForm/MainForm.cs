using FolderSyncCore;

using FolderSyncForm.Extensions;

namespace FolderSyncForm
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        private readonly FolderService _service;
        private readonly Site _site;
        private string _source => txtSource.Text;
        private string _target => txtTarget.Text;

        public MainForm()
        {
            InitializeComponent();
            _site = new Site();
            var appSettings = AppSettings.Build();
            _service = new FolderService(appSettings);
            txtSource.Text = appSettings.Source;
            txtTarget.Text = appSettings.Target;
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
            gvDiff.DataSource = _service.GetDiffFiles(_source, _target);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            _service.Backup(_source, _target);
            MessageBox.Show("複製成功，差異檔案已備份至本目錄底下backup/來源_目標/時間標記");
        }

        private void btnUpdateSite_Click(object sender, EventArgs e)
        {

            if (_site.CanClose())
            {
                _site.CloseSite(_target);
                _service.Backup(_source, _target);
                _site.OpenSite(_target);
                MessageBox.Show("站台更新成功，差異檔案已備份至本目錄底下backup/來源_目標/時間標記");
            }
            else
            {
                MessageBox.Show("找不到App_offline.htm，請將檔案放置本執行檔旁邊");
            }
        }

        private void btnBackupList_Click(object sender, EventArgs e)
        {
            gvDiff.DataSource = _service.GetBackupFolders(_source, _target);
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
            var backupDir = (FolderDTO)gvDiff.SelectedRows[0].DataBoundItem;

            _service.Restore(backupDir.完整路徑, _target);

            MessageBox.Show("還原成功");
        }
    }
}
