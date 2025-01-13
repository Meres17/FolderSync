using FolderSyncCore;

namespace FolderSyncForm
{
    internal class AppSettingsAppService
    {
        internal void Save(string source, string dest, string ignoreFolders, string ignoreFiles)
        {
            var appSettings = new AppSettings();
            appSettings.Source = source;
            appSettings.Dest = dest;
            appSettings.IgnoreFolders = ignoreFolders.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            appSettings.IgnoreFiles = ignoreFiles.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            appSettings.Save();
        }
    }
}
