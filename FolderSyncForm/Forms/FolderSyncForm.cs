
using FolderSyncForm.Extensions;
using FolderSyncForm.Helpers;

namespace FolderSyncForm.Forms
{
    public partial class FolderSyncForm : System.Windows.Forms.Form
    {
        private readonly Func<FolderSyncService> _serviceFactory = default!;
        private readonly Func<AppSettings> _settingsFactory = default!;
        private readonly Func<AppSettingsForm> _settingsFormFactory = default!;
        private FolderSyncService _service = default!;
        private string _source => txtSource.Text;
        private string _dest => txtDest.Text;
        private SyncMode _mode => cbType.SelectedItem is SyncMode mode ? mode : SyncMode.資料夾;

        /// <summary>
        /// 設計工具專用建構子，執行期請使用 DI 注入的多參數建構子。
        /// </summary>
        public FolderSyncForm()
        {
            InitializeComponent();
        }

        public FolderSyncForm(
            Func<FolderSyncService> serviceFactory,
            Func<AppSettings> settingsFactory,
            Func<AppSettingsForm> settingsFormFactory)
        {
            _serviceFactory = serviceFactory;
            _settingsFactory = settingsFactory;
            _settingsFormFactory = settingsFormFactory;

            InitializeComponent();
            InitForm();
            FormTheme.Apply(this);
        }

        private void InitForm()
        {
            var settings = _settingsFactory();
            _service = _serviceFactory();

            txtSource.Text = settings.Source;
            txtDest.Text = settings.Dest;

            cbType.DataSource = _service.GetSyncModes().ToList();
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

        private async void btnOverwrite_Click(object sender, EventArgs e)
        {
            try
            {
                SetBusy(true);
                await _service.ApplyAsync(_source, _dest, _mode);
                Dialog.Alert($"{_mode}更新成功");
                ShowCompareList();
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void btnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                SetBusy(true);
                await _service.RollbackAsync(GetSelectedBackupFolders(), _dest, _mode);
                Dialog.Alert($"{_mode}還原成功");
                ShowCompareList();
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
                ShowBackupList();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SetBusy(bool busy)
        {
            UseWaitCursor = busy;
            foreach (Control control in Controls)
            {
                if (control is Button button)
                {
                    button.Enabled = !busy;
                }
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
            var form = _settingsFormFactory();
            if (form.ShowDialog() == DialogResult.OK)
            {
                InitForm();
            }
        }
    }
}
