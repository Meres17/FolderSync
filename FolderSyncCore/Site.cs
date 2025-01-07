namespace FolderSyncCore
{
    public class Site
    {
        public bool CanClose(string? path = null)
        {
            if (path is null)
            {
                path = GetSourcePath();
            }
            return File.Exists(path);
        }

        private string GetSourcePath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "App_offline.htm");
        }

        public void CloseSite(string path)
        {
            File.Copy(GetSourcePath(), GetDestPath(path), true);
        }

        private string GetDestPath(string path)
        {
            return Path.Combine(path, "App_offline.htm");
        }

        public void OpenSite(string path)
        {
            File.Delete(GetDestPath(path));
        }

        public void ToggleSite(string dust, Action action)
        {
            if (!CanClose()) throw new Exception("找不到App_offline.htm，請將檔案放置本執行檔旁邊");
            CloseSite(dust);
            action();
            OpenSite(dust);
        }
    }
}
