using System.Text.Json;

namespace FolderSyncCore
{
    public class AppSettings
    {
        public string Source { get; set; } = "";
        public string Dest { get; set; } = "";
        public string[] IgnoreFiles { get; set; } = new[] { "appsettings.json", "web.config", "App_offline.htm" };
        public string[] IgnoreFolders { get; set; } = new[] { "logs" };

        public static AppSettings Build(string? path = null)
        {
            if (path is null)
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            }
            return BindAppSettings(path);
        }

        private static AppSettings BindAppSettings(string path)
        {
            try
            {
                var text = File.ReadAllText(path);
                return JsonSerializer.Deserialize<AppSettings>(text) ?? new AppSettings();
            }
            catch (Exception)
            {
                return new AppSettings();
            }
        }
    }
}
