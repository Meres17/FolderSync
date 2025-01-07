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

    }
}
