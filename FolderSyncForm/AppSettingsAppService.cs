using FolderSyncCore;

namespace FolderSyncForm
{
    internal class AppSettingsAppService
    {
        //無效字元
        private char[] _invalidChars = new[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

        internal void Save(string source, string dest, string ignoreFolders, string ignoreFiles, string siteDelayText)
        {
            CheckDirectory(source);
            CheckDirectory(dest);
            CheckName(ignoreFolders);
            CheckName(ignoreFiles);

            var appSettings = ToAppSettings(source, dest, ignoreFolders, ignoreFiles, siteDelayText);
            appSettings.Save();
        }

        private static AppSettings ToAppSettings(string source, string dest, string ignoreFolders, string ignoreFiles, string siteDelayText)
        {
            var appSettings = new AppSettings();
            appSettings.Source = source;
            appSettings.Dest = dest;
            appSettings.IgnoreFolders = ToArray(ignoreFolders);
            appSettings.IgnoreFiles = ToArray(ignoreFiles);
            var delay = int.TryParse(siteDelayText, out int siteDelay);
            appSettings.SiteDelay = siteDelay;
            return appSettings;
        }

        private void CheckDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (Directory.Exists(path))
            {
                return;
            }

            throw new DirectoryNotFoundException($"資料夾不存在或路徑無效：{path}");
        }

        private void CheckName(string nameString)
        {
            string[] names = ToArray(nameString);

            foreach (var invalidChar in _invalidChars)
            {
                foreach (var name in names)
                {
                    if (name.Contains(invalidChar))
                    {
                        throw new Exception($"「{name}」有無效符號「{invalidChar}」");
                    }
                }
            }
        }

        private static string[] ToArray(string nameString)
        {
            return nameString.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
