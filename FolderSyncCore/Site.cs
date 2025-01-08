namespace FolderSyncCore
{
    public class Site
    {
        public void OpenSite(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"找不到站台資料夾：{path}");
            }

            var destPath = GetDestPath(path);

            if (File.Exists(destPath))
            {
                File.Delete(destPath);
            }
        }

        public void CloseSite(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"找不到站台資料夾：{path}");
            }

            var sourcePath = GetSourcePath();

            if (!File.Exists(sourcePath))
            {
                throw new Exception("找不到App_offline.htm，請將檔案放置本執行檔旁邊");
            }

            File.Copy(sourcePath, GetDestPath(path), true);
        }
        private string GetSourcePath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "App_offline.htm");
        }

        private string GetDestPath(string path)
        {
            return Path.Combine(path, "App_offline.htm");
        }
    }
}
