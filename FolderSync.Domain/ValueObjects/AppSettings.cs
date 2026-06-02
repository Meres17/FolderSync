namespace FolderSync.Domain.ValueObjects
{
    public class AppSettings
    {
        public string Source { get; set; } = "";
        public string Dest { get; set; } = "";
        public string[] IgnoreFiles { get; set; } = new[] { "appsettings.json", "web.config", "App_offline.htm" };
        public string[] IgnoreFolders { get; set; } = new[] { "logs" };
        public int SiteDelay { get; set; }
        public int RetentionCount { get; set; } = 10; // 每組來源/目標保留的備份數，0 = 不限制

        internal static AppSettings Empty()
        {
            return new AppSettings();
        }
    }
}
