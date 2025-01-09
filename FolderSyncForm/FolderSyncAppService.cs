using FolderSyncCore;

namespace FolderSyncForm
{
    internal class FolderSyncAppService
    {
        private readonly BackupFactory _factory;
        private readonly IBackup _webSite;
        private readonly IBackup _folder;
        private readonly DictionaryComparer _dictionaryComparer;

        public FolderSyncAppService(AppSettings appSettings)
        {
            _factory = new BackupFactory(appSettings);
            _folder = _factory.Create("資料夾");
            _webSite = _factory.Create(".NET站台");
            _dictionaryComparer = new DictionaryComparer(appSettings);
        }

        public string[] GetNames()
        {
            return _factory.GetNames();
        }

        public List<FileStatus> GetDiffFiles(string sourceDir, string destDir)
        {
            return _dictionaryComparer.GetFiles(sourceDir, destDir);
        }

        public List<FolderDTO> GetBackupFolders(string sourceDir, string destDir)
        {
            return _folder.GetFolders(sourceDir, destDir);
        }

        public void RestoreSite(DataGridView gv, string destDir)
        {
            var backupDir = GetBackupDir(gv);
            _webSite.Restore(backupDir.完整路徑, destDir);
        }

        public void Restore(DataGridView gv, string destDir)
        {
            var backupDir = GetBackupDir(gv);
            _folder.Restore(backupDir.完整路徑, destDir);
        }

        private FolderDTO GetBackupDir(DataGridView gv)
        {
            if (gv.SelectedRows.Count == 0)
            {
                throw new Exception("請選擇要還原的檔案");
            }

            if (gv.SelectedRows.Count > 1)
            {
                throw new Exception("要還原的檔案只能選一個");
            }

            var result = (FolderDTO)gv.SelectedRows[0].DataBoundItem;

            if (result is null)
            {
                throw new Exception("選擇的檔案不存在");
            }

            return result;
        }


        public void UpdateSite(string sourceDir, string destDir)
        {
            var files = GetDiffFiles(sourceDir, destDir);
            _webSite.Backup(files, sourceDir, destDir);
        }

        public void Backup(string sourceDir, string destDir)
        {
            var files = GetDiffFiles(sourceDir, destDir);
            _folder.Backup(files, sourceDir, destDir);
        }

    }
}
