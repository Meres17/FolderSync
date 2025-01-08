namespace FolderSyncCore
{
    public class DictionaryComparer
    {
        private readonly AppSettings _appSettings;

        public DictionaryComparer(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public List<FileStatus> GetFiles(string sourceDir, string destDir)
        {
            var sourceDic = GetPathDictionary(sourceDir);
            var destDic = GetPathDictionary(destDir);
            return CompareDictionary(sourceDic, destDic)
                .Where(x => x.狀態 != CompareState.時間相同)
                .OrderBy(x => x.狀態)
                .ThenBy(x => x.相對路徑)
                .ToList(); ;
        }

        public Dictionary<string, string> GetPathDictionary(string dir)
        {
            if (string.IsNullOrEmpty(dir))
            {
                throw new DirectoryNotFoundException($"請輸入資料夾路徑");
            }

            if (!Directory.Exists(dir))
            {
                throw new DirectoryNotFoundException($"找不到資料夾：{dir}");
            }

            return Directory
                .EnumerateFiles(dir, "*", SearchOption.AllDirectories)
                .Select(path => new
                {
                    Path = path,
                    RelativePath = Path.GetRelativePath(dir, path)
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


        private static List<FileStatus> CompareDictionary(Dictionary<string, string> sourceDic, Dictionary<string, string> destDic)
        {
            List<FileStatus> result = new();
            foreach (var source in sourceDic)
            {
                var sourcePath = source.Key;
                var sourceFullPath = source.Value;
                var sourceFile = new FileInfo(sourceFullPath);

                if (destDic.TryGetValue(sourcePath, out var destFullPath))
                {
                    var destFile = new FileInfo(destFullPath);
                    result.Add(new FileStatus(sourcePath, sourceFile, destFile));
                    destDic.Remove(sourcePath); // 移除已經比對過的檔案
                }
                else
                {
                    result.Add(new FileStatus(sourcePath, sourceFile, null));
                }
            }

            // 剩下的檔案是 dest資料夾 中多出的檔案
            foreach (var dest in destDic)
            {
                var destPath = dest.Key;
                var destFullPath = dest.Value;
                var destFile = new FileInfo(destFullPath);
                result.Add(new FileStatus(destPath, null, destFile));
            }

            return result;
        }
    }
}
