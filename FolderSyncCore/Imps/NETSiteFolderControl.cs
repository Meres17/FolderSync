namespace FolderSyncCore.Imps
{
    internal class NETSiteFolderControl : IFolderControl
    {
        private readonly IFolderControl _backup;

        public NETSiteFolderControl(IFolderControl backup)
        {
            _backup = backup ?? throw new ArgumentNullException(nameof(backup));
        }

        public List<FolderDTO> GetFolders(string sourceDir, string destDir)
        {
            return _backup.GetFolders(sourceDir, destDir);
        }

        public void Overwrite(IEnumerable<FileStatus> files, string sourceDir, string destDir)
        {
            try
            {
                CloseSite(destDir);
                _backup.Overwrite(files, sourceDir, destDir);
            }
            finally
            {
                OpenSite(destDir);
            }
        }

        public void Restore(string backupDir, string destDir)
        {
            try
            {
                CloseSite(destDir);
                _backup.Restore(backupDir, destDir);
            }
            finally
            {
                OpenSite(destDir);
            }
        }

        private void CloseSite(string destDir)
        {
            if (!Directory.Exists(destDir))
            {
                throw new DirectoryNotFoundException($"找不到站台資料夾：{destDir}");
            }

            var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "App_offline.htm");

            if (!File.Exists(sourcePath))
            {
                throw new Exception("找不到App_offline.htm，請將檔案放置本執行檔旁邊");
            }

            File.Copy(sourcePath, GetOfflineFile(destDir), true);
        }

        private string GetOfflineFile(string destDir)
        {
            return Path.Combine(destDir, "App_offline.htm");
        }

        private void OpenSite(string destDir)
        {
            if (!Directory.Exists(destDir))
            {
                throw new DirectoryNotFoundException($"找不到站台資料夾：{destDir}");
            }

            var offlineFile = GetOfflineFile(destDir);

            if (File.Exists(offlineFile))
            {
                File.Delete(offlineFile);
            }
        }

    }
}
