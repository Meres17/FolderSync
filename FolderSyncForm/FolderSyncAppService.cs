using FolderSyncCore;

namespace FolderSyncForm
{
    internal class FolderSyncAppService
    {
        private readonly NetSiteBackup _netSiteBackup;
        private readonly AppSettings _appSettings;
        private readonly FolderBackup _filebackup;
        private readonly DictionaryComparer _dictionaryComparer;

        public FolderSyncAppService(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _filebackup = new FolderBackup(appSettings);
            _dictionaryComparer = new DictionaryComparer(appSettings);
            _netSiteBackup = new NetSiteBackup(_filebackup);
        }


        public List<FileStatus> GetDiffFiles(string sourceDir, string destDir)
        {
            return _dictionaryComparer.GetFiles(sourceDir, destDir);
        }

        public List<FolderDTO> GetBackupFolders(string sourceDir, string destDir)
        {
            return _filebackup.GetFolders(sourceDir, destDir);
        }

        public void RestoreSite(DataGridView gv, string dest)
        {
            var backupDir = GetBackupDir(gv);
            _netSiteBackup.Restore(backupDir.完整路徑, dest);
        }

        public void Restore(DataGridView gv, string destDir)
        {
            var backupDir = GetBackupDir(gv);
            _filebackup.Restore(backupDir.完整路徑, destDir);
        }

        private FolderDTO GetBackupDir(DataGridView gvDiff)
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


        public void UpdateSite(string sourceDir, string destDir)
        {
            var files = GetDiffFiles(sourceDir, destDir);
            _netSiteBackup.Backup(files, sourceDir, destDir);
        }

        public void Backup(string sourceDir, string destDir)
        {
            var files = GetDiffFiles(sourceDir, destDir);
            _filebackup.Backup(files, sourceDir, destDir);
        }

    }
}
