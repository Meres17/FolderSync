namespace FolderSyncCore
{
    public class AppSettings
    {
        public string Source { get; set; } = "";
        public string Target { get; set; } = "";
        public string[] IgnoreFiles { get; set; } = new[] { "appsettings.json", "web.config" };
        public string[] IgnoreFolders { get; set; } = new[] { "logs" };
    }
}
