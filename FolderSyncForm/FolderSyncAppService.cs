using FolderSyncCore;

namespace FolderSyncForm
{
    internal class FolderSyncAppService
    {
        private readonly BackupFactory _factory;
        private readonly IFolderReader _reader;

        public FolderSyncAppService(AppSettings appSettings)
        {
            _factory = new BackupFactory(appSettings);
            _reader = new FolderReader(appSettings);
        }

        public string[] GetNames()
        {
            return _factory.GetNames();
        }

        public List<FileStatus> GetDiffFiles(string sourceDir, string destDir)
        {
            return _reader.GetDiffFiles(sourceDir, destDir);
        }

        public List<FolderDTO> GetBackupFolders(string sourceDir, string destDir, string type)
        {
            var backup = _factory.Create(type);
            return backup.GetFolders(sourceDir, destDir);
        }

        public void Restore(DataGridView gv, string destDir, string type)
        {
            var backupDir = GetBackupDir(gv);
            var backup = _factory.Create(type);
            backup.Restore(backupDir.完整路徑, destDir);
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

        public void Overwrite(string sourceDir, string destDir, string type)
        {
            var files = GetDiffFiles(sourceDir, destDir);
            var backup = _factory.Create(type);
            backup.Backup(files, sourceDir, destDir);
        }

    }
}
