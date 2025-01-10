namespace FolderSyncCore.Imps
{
    internal class NETSiteFolderControl : IFolderControl
    {
        private readonly IFolderControl _folderControl;

        public NETSiteFolderControl(IFolderControl folderControl)
        {
            _folderControl = folderControl ?? throw new ArgumentNullException(nameof(folderControl));
        }

        public List<FolderDTO> GetFolders(string sourceDir, string destDir)
        {
            return _folderControl.GetFolders(sourceDir, destDir);
        }

        public void Overwrite(IEnumerable<FileStatus> files, string sourceDir, string destDir)
        {
            try
            {
                CloseSite(destDir);
                _folderControl.Overwrite(files, sourceDir, destDir);
            }
            finally
            {
                OpenSite(destDir);
            }
        }

        internal virtual void CloseSite(string destDir)
        {
            if (NotFoundDirectory(destDir))
            {
                throw new DirectoryNotFoundException($"找不到站台資料夾：{destDir}");
            }

            var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "App_offline.htm");

            if (NotFoundFile(sourcePath))
            {
                throw new Exception("找不到App_offline.htm，請將檔案放置本執行檔旁邊");
            }

            Copy(destDir, sourcePath);
        }

        public void Restore(string backupDir, string destDir)
        {
            try
            {
                CloseSite(destDir);
                _folderControl.Restore(backupDir, destDir);
            }
            finally
            {
                OpenSite(destDir);
            }
        }

        internal virtual bool NotFoundDirectory(string destDir)
        {
            return !Directory.Exists(destDir);
        }

        internal virtual bool NotFoundFile(string sourcePath)
        {
            return !File.Exists(sourcePath);
        }

        internal virtual void Copy(string destDir, string sourcePath)
        {
            File.Copy(sourcePath, GetOfflineFile(destDir), true);
        }

        private string GetOfflineFile(string destDir)
        {
            return Path.Combine(destDir, "App_offline.htm");
        }

        internal virtual void OpenSite(string destDir)
        {
            if (NotFoundDirectory(destDir))
            {
                throw new DirectoryNotFoundException($"找不到站台資料夾：{destDir}");
            }

            var offlineFile = GetOfflineFile(destDir);

            if (NotFoundFile(offlineFile)) return;//沒有此檔案代表站台已經開啟

            Delete(offlineFile);
        }

        internal virtual void Delete(string offlineFile)
        {
            File.Delete(offlineFile);
        }
    }
}
