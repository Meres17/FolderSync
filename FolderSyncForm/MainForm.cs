using FolderSyncCore;

using FolderSyncForm.Extensions;

namespace FolderSyncForm
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        private readonly FolderSyncAppService _service;
        private string _source => txtSource.Text;
        private string _dest => txtDest.Text;

        public MainForm()
        {
            InitializeComponent();
            var appSettings = AppSettings.Build();
            _service = new FolderSyncAppService(appSettings);
            txtSource.Text = appSettings.Source;
            txtDest.Text = appSettings.Target;
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            txtSource.BindFolderDialog();
        }

        private void btnDest_Click(object sender, EventArgs e)
        {
            txtDest.BindFolderDialog();
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            gv.DataSource = _service.GetDiffFiles(_source, _dest);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            _service.Backup(_source, _dest);
            MessageBox.Show("複製成功");
        }

        private void btnUpdateSite_Click(object sender, EventArgs e)
        {
            try
            {
                _service.ToggleSite(_dest, () => _service.Backup(_source, _dest));
                MessageBox.Show("站台更新成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBackupList_Click(object sender, EventArgs e)
        {
            gv.DataSource = _service.GetBackupFolders(_source, _dest);
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                var backupDir = _service.GetBackupDir(gv);
                _service.Restore(backupDir.完整路徑, _dest);
                MessageBox.Show("還原成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRestoreSite_Click(object sender, EventArgs e)
        {
            try
            {
                var backupDir = _service.GetBackupDir(gv);
                _service.ToggleSite(_dest, () => _service.Restore(backupDir.完整路徑, _dest));
                MessageBox.Show("站台還原成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

    }
}
