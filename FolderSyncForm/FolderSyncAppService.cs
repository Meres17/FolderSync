using FolderSyncCore;

namespace FolderSyncForm
{
    internal class FolderSyncAppService
    {
        private readonly FolderSyncCore.Site _site;
        private readonly AppSettings _appSettings;
        private readonly FileBackup _filebackup;
        private readonly DictionaryComparer _dictionaryComparer;

        public FolderSyncAppService(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _filebackup = new FileBackup(appSettings);
            _dictionaryComparer = new DictionaryComparer(appSettings);
            _site = new FolderSyncCore.Site();
        }

        public void Backup(string sourceDir, string destDir)
        {
            var files = GetDiffFiles(sourceDir, destDir);
            _filebackup.Backup(files, sourceDir, destDir);
        }

        public List<FileStatus> GetDiffFiles(string sourceDir, string destDir)
        {
            return _dictionaryComparer.GetFiles(sourceDir, destDir);
        }

        public List<FolderDTO> GetBackupFolders(string sourceDir, string destDir)
        {
            return _filebackup.GetFolders(sourceDir, destDir);
        }
        public void Restore(string backupDir, string destDir)
        {
            _filebackup.Restore(backupDir, destDir);
        }

        public void ToggleSite(string dest, Action action)
        {
            if (!_site.CanClose()) throw new Exception("找不到App_offline.htm，請將檔案放置本執行檔旁邊");
            _site.CloseSite(dest);
            action();
            _site.OpenSite(dest);
        }

        public FolderDTO GetBackupDir(DataGridView gvDiff)
        {
            if (gvDiff.SelectedRows.Count == 0)
            {
                throw new Exception("請選擇要還原的檔案");
            }

            if (gvDiff.SelectedRows.Count > 1)
            {
                throw new Exception("要還原的檔案只能選一個");
            }

            var result = (FolderDTO)gvDiff.SelectedRows[0].DataBoundItem;

            if (result is null)
            {
                throw new Exception("選擇的檔案不存在");
            }

            return result;
        }
    }
}
