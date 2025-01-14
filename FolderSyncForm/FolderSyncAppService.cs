using FolderSyncCore;

using FolderSyncForm.Helpers;

namespace FolderSyncForm
{
    internal class FolderSyncAppService
    {
        private readonly FolderFactory _factory;

        public FolderSyncAppService(AppSettings appSettings)
        {
            _factory = new FolderFactory(appSettings);
        }

        public string[] GetNames()
        {
            return _factory.GetNames();
        }

        public List<FileStatus> GetDiffFiles(string sourceDir, string destDir)
        {
            return _factory.GetDiffFiles(sourceDir, destDir);
        }

        public List<FolderDTO> GetBackupFolders(string sourceDir, string destDir)
        {
            return _factory.GetBackupFolders(sourceDir, destDir);
        }

        public void Restore(DataGridView gv, string destDir, string type)
        {
            var backupFolders = GetBackupFolders(gv);

            if (backupFolders.Count > 1)
            {
                throw new Exception("要還原的檔案只能選一個");
            }

            var backupDir = backupFolders.First();
            var folderControl = _factory.CreateControl(type);
            folderControl.Restore(backupDir.完整路徑, destDir);
        }

        public void Overwrite(string sourceDir, string destDir, string type)
        {
            var files = GetDiffFiles(sourceDir, destDir);
            var folderControl = _factory.CreateControl(type);
            folderControl.Overwrite(files, sourceDir, destDir);
        }

        public void DeleteBackup(DataGridView gv)
        {
            var backupFolders = GetBackupFolders(gv);

            if (backupFolders.Count == 0)
            {
                throw new Exception("選擇的備份紀錄不存在");
            }

            if (Dialog.Confirm("確定刪除備份紀錄嗎"))
            {
                backupFolders.ForEach(x => Directory.Delete(x.完整路徑, true));
            }
        }

        private static List<FolderDTO?> GetBackupFolders(DataGridView gv)
        {
            if (gv.SelectedRows.Count == 0)
            {
                throw new Exception("請選擇備份紀錄");
            }

            return gv.SelectedRows
                .OfType<DataGridViewRow>()
                .Select(x => x.DataBoundItem as FolderDTO)
                .Where(x => x != null)
                .ToList();
        }
    }
}
