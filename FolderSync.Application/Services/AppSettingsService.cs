
namespace FolderSync.Application.Services
{
    public class AppSettingsService
    {
        //無效字元
        private readonly char[] _invalidChars = new[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
        private readonly IAppSettingsRepository _repository;
        private readonly AppSettingsProvider _provider;
        private readonly IDirectoryProbe _directories;

        public AppSettingsService(IAppSettingsRepository repository, AppSettingsProvider provider, IDirectoryProbe directories)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _directories = directories ?? throw new ArgumentNullException(nameof(directories));
        }

        public void Save(string source, string dest, string ignoreFolders, string ignoreFiles, string siteDelayText)
        {
            ValidateDirectoryExists(source);
            ValidateDirectoryExists(dest);
            ValidateNoInvalidChars(ignoreFolders);
            ValidateNoInvalidChars(ignoreFiles);

            var appSettings = BuildSettings(source, dest, ignoreFolders, ignoreFiles, siteDelayText);
            _repository.Save(appSettings);
            _provider.Reload();
        }

        private AppSettings BuildSettings(string source, string dest, string ignoreFolders, string ignoreFiles, string siteDelayText)
        {
            var appSettings = _repository.Load();
            appSettings.Source = source;
            appSettings.Dest = dest;
            appSettings.IgnoreFolders = SplitLines(ignoreFolders);
            appSettings.IgnoreFiles = SplitLines(ignoreFiles);
            int.TryParse(siteDelayText, out int siteDelay);
            appSettings.SiteDelay = siteDelay;
            return appSettings;
        }

        private void ValidateDirectoryExists(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (_directories.Exists(path))
            {
                return;
            }

            throw new DirectoryNotFoundException($"資料夾不存在或路徑無效：{path}");
        }

        private void ValidateNoInvalidChars(string nameString)
        {
            string[] names = SplitLines(nameString);

            foreach (var invalidChar in _invalidChars)
            {
                foreach (var name in names)
                {
                    if (name.Contains(invalidChar))
                    {
                        throw new DomainException($"「{name}」有無效符號「{invalidChar}」");
                    }
                }
            }
        }

        private static string[] SplitLines(string nameString)
        {
            return nameString.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
