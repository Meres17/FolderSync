using FolderSyncForm.Extensions;
using FolderSyncForm.Helpers;

namespace FolderSyncForm.Forms
{
    public partial class FolderSyncForm : System.Windows.Forms.Form
    {
        private readonly FolderSyncService _service = default!;
        private readonly AppSettingsProvider _settings = default!;
        private readonly AppSettingsService _settingsService = default!;
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

        public FolderSyncForm(FolderSyncService service, AppSettingsProvider settings, AppSettingsService settingsService)
        {
            _service = service;
            _settings = settings;
            _settingsService = settingsService;

            InitializeComponent();
            cbType.DataSource = _service.GetSyncModes().ToList();
            cbType.SelectedItem = SyncMode.站台;
            RefreshFromSettings();
            FormTheme.Apply(this);
        }

        private void RefreshFromSettings()
        {
            txtSource.Text = _settings.Current.Source;
            txtDest.Text = _settings.Current.Dest;
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            txtSource.BindFolderDialog();
        }

        private void btnDest_Click(object sender, EventArgs e)
        {
            txtDest.BindFolderDialog();
        }

        private async void btnCompare_Click(object sender, EventArgs e)
        {
            SetBusy(true);
            try
            {
                await ShowCompareListAsync();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void btnBackupList_Click(object sender, EventArgs e)
        {
            SetBusy(true);
            try
            {
                await ShowBackupListAsync();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void btnOverwrite_Click(object sender, EventArgs e)
        {
            SetBusy(true);
            try
            {
                string source = _source, dest = _dest;
                SyncMode mode = _mode;
                if (await ConfirmOverwriteAsync(source, dest, mode) == false)
                    return;

                await Task.Run(() => _service.ApplyAsync(source, dest, mode));
                Dialog.Alert($"{mode}更新成功");
                await ShowCompareListAsync();
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

        // 更新前列出「有差異但符合忽略規則」的檔案，讓使用者做最終確認。
        private async Task<bool> ConfirmOverwriteAsync(string source, string dest, SyncMode mode)
        {
            var skipped = await Task.Run(() => _service.GetIgnoredDiffFiles(source, dest));
            var message = skipped.Count == 0
                ? $"確定要執行{mode}更新嗎？\n沒有符合忽略規則而略過的檔案。"
                : $"確定要執行{mode}更新嗎？\n以下 {skipped.Count} 個檔案符合忽略規則，不會被覆蓋：";
            var lines = skipped.Select(x => $"[{x.狀態}] {x.相對路徑}");
            return Dialog.ConfirmList(message, lines);
        }

        private async void btnRestore_Click(object sender, EventArgs e)
        {
            SetBusy(true);
            try
            {
                var selected = GetSelectedBackupFolders();
                string dest = _dest;
                SyncMode mode = _mode;
                await Task.Run(() => _service.RollbackAsync(selected, dest, mode));
                Dialog.Alert($"{mode}還原成功");
                await ShowCompareListAsync();
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
                await ShowBackupListAsync();
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

        private async void btnDeleteBackup_Click(object sender, EventArgs e)
        {
            SetBusy(true);
            try
            {
                var selected = GetSelectedBackupFolders();
                if (Dialog.Confirm("確定刪除備份紀錄嗎"))
                {
                    await Task.Run(() => _service.DeleteBackups(selected));
                    Dialog.Alert("刪除成功");
                }
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }
            finally
            {
                await ShowBackupListAsync();
                SetBusy(false);
            }
        }

        // 掃描可能很慢，卸載到背景執行緒避免凍結 UI；輸入先在 UI 執行緒擷取，避免跨緒存取控制項。
        private async Task ShowCompareListAsync()
        {
            string source = _source, dest = _dest;
            try
            {
                gv.DataSource = await Task.Run(() => _service.GetDiffFiles(source, dest));
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }
        }

        private async Task ShowBackupListAsync()
        {
            string source = _source, dest = _dest;
            try
            {
                gv.DataSource = await Task.Run(() => _service.GetBackupFolders(source, dest));
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
                throw new DomainException("請選擇備份紀錄");
            }

            return gv.SelectedRows
                .OfType<DataGridViewRow>()
                .Select(x => x.DataBoundItem)
                .OfType<BackupFolder>()
                .ToList();
        }

        private void btnOpenSettings_Click(object sender, EventArgs e)
        {
            using var form = new AppSettingsForm(_settingsService, _settings);
            if (form.ShowDialog() == DialogResult.OK)
            {
                RefreshFromSettings();
            }
        }
    }
}
