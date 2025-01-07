namespace FolderSyncCore
{
    internal class DictionaryComparer
    {
        private readonly Dictionary<string, string> _sourceDic;
        private readonly Dictionary<string, string> _targetDic;
        private readonly AppSettings _appSettings;

        public DictionaryComparer(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public List<FileStatus> GetFiles(string sourceDir, string targetDir)
        {
            var sourceDic = GetPathDictionary(sourceDir);
            var targetDic = GetPathDictionary(targetDir);
            return CompareDictionary(sourceDic, targetDic)
                .Where(x => x.狀態 != CompareState.時間相同)
                .OrderBy(x => x.狀態)
                .ThenBy(x => x.相對路徑)
                .ToList(); ;
        }

        public Dictionary<string, string> GetPathDictionary(string source)
        {
            return Directory
                .EnumerateFiles(source, "*", SearchOption.AllDirectories)
                .Select(path => new
                {
                    Path = path,
                    RelativePath = Path.GetRelativePath(source, path)
                })
                .ToList()
                .Where(x => !IsIgnoreFile(x.Path, _appSettings.IgnoreFiles))
                .Where(x => !IsInFolder(x.RelativePath, _appSettings.IgnoreFolders))
                .ToDictionary(x => x.RelativePath, x => x.Path);
        }

        private static bool IsIgnoreFile(string path, params string[] excludedFiles)
        {
            return excludedFiles.Any(excludedFile => path.EndsWith(excludedFile, StringComparison.InvariantCultureIgnoreCase));
        }

        private static bool IsInFolder(string relativePath, params string[] dirs)
        {
            var directoryName = Path.GetDirectoryName(relativePath);
            if (string.IsNullOrEmpty(directoryName))
            {
                return false;
            }
            return dirs.Any(dir => IsInDirectory(directoryName, dir));
        }

        private static bool IsInDirectory(string directoryName, string dir)
        {
            return directoryName
                .Split(Path.DirectorySeparatorChar)
                .Contains(dir, StringComparer.InvariantCultureIgnoreCase);
        }


        private static List<FileStatus> CompareDictionary(Dictionary<string, string> sourceDictionary, Dictionary<string, string> targetDictionary)
        {
            List<FileStatus> result = new();
            foreach (var source in sourceDictionary)
            {
                var sourcePath = source.Key;
                var sourceFullPath = source.Value;
                var sourceFile = new FileInfo(sourceFullPath);

                if (targetDictionary.TryGetValue(sourcePath, out var targetFullPath))
                {
                    var targetFile = new FileInfo(targetFullPath);
                    result.Add(new FileStatus(sourcePath, sourceFile, targetFile));
                    targetDictionary.Remove(sourcePath); // 移除已經比對過的檔案
                }
                else
                {
                    result.Add(new FileStatus(sourcePath, sourceFile, null));
                }
            }

            // 剩下的檔案是 targetFolder 中多出的檔案
            foreach (var target in targetDictionary)
            {
                var targetPath = target.Key;
                var targetFullPath = target.Value;
                var targetFile = new FileInfo(targetFullPath);
                result.Add(new FileStatus(targetPath, null, targetFile));
            }

            return result;
        }
    }
}
