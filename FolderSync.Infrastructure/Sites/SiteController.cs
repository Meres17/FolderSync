namespace FolderSync.Infrastructure.Sites
{
    internal class SiteController : ISiteController
    {
        private const string OfflineFileName = "App_offline.htm";

        public void TakeOffline(string siteDir)
        {
            EnsureSiteExists(siteDir);

            var source = Path.Combine(Directory.GetCurrentDirectory(), OfflineFileName);
            if (FileMissing(source))
            {
                throw new FileNotFoundException("找不到App_offline.htm，請將檔案放置本執行檔旁邊");
            }

            CopyOfflineFile(siteDir, source);
        }

        public void BringOnline(string siteDir)
        {
            EnsureSiteExists(siteDir);

            var offlineFile = GetOfflineFile(siteDir);
            if (FileMissing(offlineFile)) return;//沒有此檔案代表站台已經開啟

            DeleteOfflineFile(offlineFile);
        }

        private void EnsureSiteExists(string siteDir)
        {
            if (DirectoryMissing(siteDir))
            {
                throw new DirectoryNotFoundException($"找不到站台資料夾：{siteDir}");
            }
        }

        internal virtual bool DirectoryMissing(string siteDir)
        {
            return !Directory.Exists(siteDir);
        }

        internal virtual bool FileMissing(string path)
        {
            return !File.Exists(path);
        }

        internal virtual void CopyOfflineFile(string siteDir, string source)
        {
            File.Copy(source, GetOfflineFile(siteDir), true);
        }

        internal virtual void DeleteOfflineFile(string offlineFile)
        {
            File.Delete(offlineFile);
        }

        private static string GetOfflineFile(string siteDir)
        {
            return Path.Combine(siteDir, OfflineFileName);
        }
    }
}
