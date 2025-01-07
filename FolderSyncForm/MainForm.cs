using FolderSyncCore;

using FolderSyncForm.Extensions;

namespace FolderSyncForm
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        private readonly FolderService _service;
        private readonly Site _site;
        private string _source => txtSource.Text;
        private string _dest => txtDest.Text;

        public MainForm()
        {
            InitializeComponent();
            _site = new Site();
            var appSettings = AppSettings.Build();
            _service = new FolderService(appSettings);
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
            gvDiff.DataSource = _service.GetDiffFiles(_source, _dest);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            _service.Backup(_source, _dest);
            MessageBox.Show("�ƻs���\");
        }

        private void btnUpdateSite_Click(object sender, EventArgs e)
        {
            try
            {
                _site.ToggleSite(_dest, () => _service.Backup(_source, _dest));
                MessageBox.Show("���x��s���\");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBackupList_Click(object sender, EventArgs e)
        {
            gvDiff.DataSource = _service.GetBackupFolders(_source, _dest);
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            // ���o����� FolderDTO ����
            var backupDir = GetSelectedDirectory();
            if (backupDir is null) return;
            _service.Restore(backupDir.������|, _dest);
            MessageBox.Show("�٭즨�\");
        }

        private void btnRestoreSite_Click(object sender, EventArgs e)
        {
            try
            {
                var backupDir = GetSelectedDirectory();
                if (backupDir is null) return;
                _site.ToggleSite(_dest, () => _service.Restore(backupDir.������|, _dest));
                MessageBox.Show("���x�٭즨�\");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private FolderDTO? GetSelectedDirectory()
        {
            if (gvDiff.SelectedRows.Count == 0)
            {
                MessageBox.Show("�п�ܭn�٭쪺�ɮ�");
                return null;
            }

            if (gvDiff.SelectedRows.Count > 1)
            {
                MessageBox.Show("�n�٭쪺�ɮץu���@��");
                return null;
            }

            return (FolderDTO)gvDiff.SelectedRows[0].DataBoundItem;
        }
    }
}
