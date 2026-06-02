namespace FolderSync.Domain
{
    public class AppSettings
    {
        public string Source { get; set; } = "";
        public string Dest { get; set; } = "";
        public string[] IgnoreFiles { get; set; } = new[] { "appsettings.json", "web.config", "App_offline.htm" };
        public string[] IgnoreFolders { get; set; } = new[] { "logs" };
        public int SiteDelay { get; set; }

        internal static AppSettings Empty()
        {
            return new AppSettings();
        }
    }
}
