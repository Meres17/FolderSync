using FolderSyncCore;

using FolderSyncForm.Extensions;

namespace FolderSyncForm
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        private readonly FolderSyncAppService _service;
        private string _source => txtSource.Text;
        private string _dest => txtDest.Text;
        private string _type => cbType.Text;

        public MainForm()
        {
            InitializeComponent();
            var appSettings = AppSettings.Build();
            txtSource.Text = appSettings.Source;
            txtDest.Text = appSettings.Target;

            _service = new FolderSyncAppService(appSettings);
            cbType.Items.AddRange(_service.GetNames());
            cbType.SelectedIndex = 1;
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
            try
            {
                gv.DataSource = _service.GetDiffFiles(_source, _dest);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                _service.Backup(_source, _dest, _type);
                MessageBox.Show(_type + "更新成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdateSite_Click(object sender, EventArgs e)
        {
            try
            {
                _service.Backup(_source, _dest, _type);
                MessageBox.Show(_type + "更新成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBackupList_Click(object sender, EventArgs e)
        {
            gv.DataSource = _service.GetBackupFolders(_source, _dest, _type);
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                _service.Restore(gv, _dest, _type);
                MessageBox.Show(_type + "還原成功");
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
                _service.Restore(gv, _dest, _type);
                MessageBox.Show(_type + "還原成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var isFolder = cbType.Text?.Contains("資料夾") == true;
            btnCopy.Visible = isFolder;
            btnRestore.Visible = isFolder;

            btnUpdateSite.Visible = !isFolder;
            btnRestoreSite.Visible = !isFolder;
        }
    }
}
