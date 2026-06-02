using FolderSync.Application;
using FolderSync.Domain;
using FolderSync.Infrastructure;

using FolderSyncForm.Extensions;
using FolderSyncForm.Helpers;

namespace FolderSyncForm
{
    public partial class FolderSyncForm : System.Windows.Forms.Form
    {
        private FolderSyncService _service = default!;
        private string _source => txtSource.Text;
        private string _dest => txtDest.Text;
        private string _type => cbType.Text;

        public FolderSyncForm()
        {
            InitializeComponent();
            InitForm();
            FormTheme.Apply(this);
        }

        private void InitForm()
        {
            var settings = new JsonAppSettingsRepository().Load();
            var reader = new FolderReader(settings);
            var factory = new FolderControlFactory(reader, settings);
            _service = new FolderSyncService(reader, factory);

            txtSource.Text = settings.Source;
            txtDest.Text = settings.Dest;

            cbType.Items.Clear();
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
                _service.Restore(GetSelectedBackupFolders(), _dest, _type);
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
                var selected = GetSelectedBackupFolders();
                if (Dialog.Confirm("確定刪除備份紀錄嗎"))
                {
                    _service.DeleteBackups(selected);
                    Dialog.Alert("刪除成功");
                }
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

        private List<BackupFolder> GetSelectedBackupFolders()
        {
            if (gv.SelectedRows.Count == 0)
            {
                throw new Exception("請選擇備份紀錄");
            }

            return gv.SelectedRows
                .OfType<DataGridViewRow>()
                .Select(x => x.DataBoundItem as BackupFolder)
                .Where(x => x != null)
                .Select(x => x!)
                .ToList();
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
