using FolderSyncCore;

using FolderSyncForm.Extensions;
using FolderSyncForm.Helpers;

namespace FolderSyncForm
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        private FolderSyncAppService _service = default!;
        private string _source => txtSource.Text;
        private string _dest => txtDest.Text;
        private string _type => cbType.Text;

        public MainForm()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            var appSettings = AppSettings.Build();
            _service = new FolderSyncAppService(appSettings);

            txtSource.Text = appSettings.Source;
            txtDest.Text = appSettings.Dest;

            cbType.Items.AddRange(_service.GetNames());
            cbType.SelectedIndex = 0;
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
            ShowCompareList();
        }

        private void btnBackupList_Click(object sender, EventArgs e)
        {
            ShowBackupList();
        }

        private void btnOverwrite_Click(object sender, EventArgs e)
        {
            try
            {
                _service.Overwrite(_source, _dest, _type);
                Dialog.Alert(_type + "更新成功");
                ShowCompareList();
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                _service.Restore(gv, _dest, _type);
                Dialog.Alert(_type + "還原成功");
                ShowCompareList();
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
                ShowBackupList();
            }
        }

        private void btnDeleteBackup_Click(object sender, EventArgs e)
        {
            try
            {
                _service.DeleteBackup(gv);
                Dialog.Alert("刪除成功");
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }
            finally
            {
                ShowBackupList();
            }
        }

        private void ShowCompareList()
        {
            try
            {
                gv.DataSource = _service.GetDiffFiles(_source, _dest);
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }

        }

        private void ShowBackupList()
        {
            try
            {
                gv.DataSource = _service.GetBackupFolders(_source, _dest);
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }
        }

        private void btnOpenSettings_Click(object sender, EventArgs e)
        {
            var form = new AppSettingsForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                InitForm();
            }
        }
    }
}
