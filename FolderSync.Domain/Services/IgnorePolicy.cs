namespace FolderSync.Domain.Services
{
    /// <summary>
    /// 判斷檔案是否應被忽略（不納入比對）的領域規則。
    /// </summary>
    public class IgnorePolicy
    {
        private readonly string[] _ignoreFiles;
        private readonly string[] _ignoreFolders;

        public IgnorePolicy(string[] ignoreFiles, string[] ignoreFolders)
        {
            _ignoreFiles = ignoreFiles ?? Array.Empty<string>();
            _ignoreFolders = ignoreFolders ?? Array.Empty<string>();
        }

        public static IgnorePolicy From(AppSettings settings)
        {
            return new IgnorePolicy(settings.IgnoreFiles, settings.IgnoreFolders);
        }

        public bool ShouldIgnore(string relativePath)
        {
            return IsIgnoredFile(relativePath) || IsUnderIgnoredFolder(relativePath);
        }

        private bool IsIgnoredFile(string relativePath)
        {
            return _ignoreFiles.Any(name => relativePath.EndsWith(name, StringComparison.InvariantCultureIgnoreCase));
        }

        private bool IsUnderIgnoredFolder(string relativePath)
        {
            var directory = Path.GetDirectoryName(relativePath);
            if (string.IsNullOrEmpty(directory))
            {
                return false;
            }

            var segments = directory.Split(Path.DirectorySeparatorChar);
            return _ignoreFolders.Any(folder => segments.Contains(folder, StringComparer.InvariantCultureIgnoreCase));
        }
    }
}
