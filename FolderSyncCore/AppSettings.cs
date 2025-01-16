using System.Text.Json;

namespace FolderSyncCore
{
    public class AppSettings
    {
        public string Source { get; set; } = "";
        public string Dest { get; set; } = "";
        public string[] IgnoreFiles { get; set; } = new[] { "appsettings.json", "web.config", "App_offline.htm" };
        public string[] IgnoreFolders { get; set; } = new[] { "logs" };
        public int SiteDelay { get; set; }

        public static AppSettings Build(string? path = null)
        {
            if (path is null)
            {
                path = GetPath();
            }
            return BindAppSettings(path);
        }

        private static string GetPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        }

        private static AppSettings BindAppSettings(string path)
        {
            try
            {
                var text = File.ReadAllText(path);
                return JsonSerializer.Deserialize<AppSettings>(text) ?? Empty();
            }
            catch
            {
                return Empty();
            }
        }

        internal static AppSettings Empty()
        {
            return new AppSettings();
        }

        public void Save(string? path = null)
        {
            if (path is null)
            {
                path = GetPath();
            }

            var text = JsonSerializer.Serialize(this);

            File.WriteAllText(path, text);
        }
    }
}
