namespace FolderSyncCore.Imps
{
    internal class FolderReader : IFolderReader
    {
        private readonly AppSettings _appSettings;

        public FolderReader(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public List<FileStatus> GetDiffFiles(string sourceDir, string destDir)
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

        internal bool IsIgnoreFile(string path, params string[] excludedFiles)
        {
            return excludedFiles.Any(excludedFile => path.EndsWith(excludedFile, StringComparison.InvariantCultureIgnoreCase));
        }

        internal bool IsInFolder(string relativePath, params string[] keys)
        {
            var directory = Path.GetDirectoryName(relativePath);
            if (string.IsNullOrEmpty(directory))
            {
                return false;
            }
            return keys.Any(key => IsInDirectory(directory, key));
        }

        internal bool IsInDirectory(string directory, string key)
        {
            return directory
                .Split(Path.DirectorySeparatorChar)
                .Contains(key, StringComparer.InvariantCultureIgnoreCase);
        }


        internal List<FileStatus> CompareDictionary(Dictionary<string, string> sourceDic, Dictionary<string, string> destDic)
        {
            var result = new List<FileStatus>();
            foreach (var (sourcePath, sourceFullPath) in sourceDic)
            {
                // 移除已比對的項目，同時將找到的值傳給 destFullPath
                // destFullPath 可以為 null，表示 dest 資料夾中沒有這個檔案
                destDic.Remove(sourcePath, out var destFullPath);
                result.Add(new FileStatus(sourcePath, sourceFullPath, destFullPath));
            }

            // 剩餘的 destDic 即為 dest 資料夾中多出的檔案
            var remaining = destDic
                .Select(dest => new FileStatus(dest.Key, null, dest.Value));

            result.AddRange(remaining);

            return result;
        }
    }
}
