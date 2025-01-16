namespace FolderSyncCore.Imps
{
    internal class SiteControl : ISiteControl
    {
        private readonly AppSettings _appSettings;

        public SiteControl(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }
        public void CloseSite(string destDir)
        {
            if (NotFoundDirectory(destDir))
            {
                throw new DirectoryNotFoundException($"找不到站台資料夾：{destDir}");
            }

            var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "App_offline.htm");

            if (NotFoundFile(sourcePath))
            {
                throw new FileNotFoundException("找不到App_offline.htm，請將檔案放置本執行檔旁邊");
            }

            Copy(destDir, sourcePath);

            Thread.Sleep(_appSettings.SiteDelay);
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

        public void OpenSite(string destDir)
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
