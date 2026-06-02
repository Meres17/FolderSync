using System.Text.Json;

using FolderSync.Domain;

namespace FolderSync.Infrastructure
{
    public class JsonAppSettingsRepository : IAppSettingsRepository
    {
        private const string FileName = "appsettings.json";
        private readonly string _path;

        public JsonAppSettingsRepository(string? path = null)
        {
            _path = path ?? Path.Combine(Directory.GetCurrentDirectory(), FileName);
        }

        public AppSettings Load()
        {
            try
            {
                var text = File.ReadAllText(_path);
                return JsonSerializer.Deserialize<AppSettings>(text) ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public void Save(AppSettings settings)
        {
            var text = JsonSerializer.Serialize(settings);
            File.WriteAllText(_path, text);
        }
    }
}
