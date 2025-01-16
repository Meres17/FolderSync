using System.Globalization;

namespace FolderSyncCore.Imps
{
    internal class FolderReader : IFolderReader
    {
        private const string Format = "yyyyMMdd_HHmm";
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

        internal Dictionary<string, string> GetPathDictionary(string dir)
        {
            if (string.IsNullOrEmpty(dir))
            {
                throw new ArgumentNullException($"請輸入資料夾路徑");
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

        public List<FileStatus> GetBackupFiles(string host, string name)
        {
            var backupDir = Path.Combine(host, name);
            try
            {
                return GetPathDictionary(backupDir)
                    .Select(x => new FileStatus(x.Key, x.Value, null))
                    .ToList();
            }
            catch (DirectoryNotFoundException) //還原時，未必會有新/刪/修資料夾
            {
                return new List<FileStatus>();
            }
        }

        public List<FolderDTO> GetBackupFolders(string sourceDir, string destDir)
        {
            var backupHost = BuildBackupHost(sourceDir, destDir);
            return Directory
                .EnumerateDirectories(backupHost, "*", SearchOption.TopDirectoryOnly)
                .Where(x => DateTime.TryParseExact(Path.GetFileName(x), Format, null, DateTimeStyles.None, out _))
                .Select(x => new FolderDTO(x))
                .ToList();
        }

        private string BuildBackupHost(string sourceDir, string destDir)
        {
            var host = Directory.GetCurrentDirectory();
            var sourceFileName = Path.GetFileName(sourceDir);
            var destFileName = Path.GetFileName(destDir);
            var result = Path.Combine(host, "Backup", $"{sourceFileName}_{destFileName}");
            CreateDirectory(result);
            return result;
        }

        private static void CreateDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public string CreateBackupDirectory(string sourceDir, string destDir)
        {
            var host = BuildBackupHost(sourceDir, destDir);
            var time = DateTime.Now.ToString(Format);
            return Path.Combine(host, time);
        }
    }
}
