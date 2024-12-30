
using System.Globalization;

namespace FolderSyncCore
{
    public class FolderComparer
    {
        private const string Format = "yyyyMMdd_HHmm";
        private readonly string _sourceFolder;
        private readonly string _targetFolder;

        public FolderComparer(string sourceFolder, string targetFolder)
        {
            _sourceFolder = sourceFolder;
            _targetFolder = targetFolder;
        }

        public List<FileDTO> GetDiffFiles()
        {
            return GetDiff(_sourceFolder, _targetFolder);
        }

        private static List<FileDTO> GetDiff(string sourceFolder, string targetFolder)
        {
            List<FileDTO> result = new();

            var sourceDic = GetDictionary(sourceFolder);
            var targetDic = GetDictionary(targetFolder);

            // 比較檔案
            foreach (var source in sourceDic)
            {
                var sourcePath = source.Key;
                var sourceFullPath = source.Value;
                var sourceFile = new FileInfo(sourceFullPath);

                if (targetDic.TryGetValue(sourcePath, out var targetFullPath))
                {
                    var targetFile = new FileInfo(targetFullPath);
                    result.Add(new FileDTO(sourcePath, sourceFile, targetFile));
                    targetDic.Remove(sourcePath); // 移除已經比對過的檔案
                }
                else
                {
                    result.Add(new FileDTO(sourcePath, sourceFile, null));
                }
            }

            // 剩下的檔案是 targetFolder 中多出的檔案
            foreach (var target in targetDic)
            {
                var targetPath = target.Key;
                var targetFullPath = target.Value;
                var targetFile = new FileInfo(targetFullPath);
                result.Add(new FileDTO(targetPath, null, targetFile));
            }

            return result
                .Where(x => x.狀態 != CompareState.時間相同)
                .OrderBy(x => x.狀態)
                .ThenBy(x => x.相對路徑)
                .ToList();
        }

        private static Dictionary<string, string> GetDictionary(string source)
        {
            return Directory
                .EnumerateFiles(source, "*", SearchOption.AllDirectories)
                .Where(path => !path.EndsWith("appsettings.json"))
                .Where(path => !path.EndsWith("web.config"))
                .Select(path => new
                {
                    path,
                    RelativePath = Path.GetRelativePath(source, path)
                })
                .ToList()
                .Where(x => !IsInFolder(x.RelativePath, "logs"))
                .ToDictionary(x => x.RelativePath, x => x.path);
        }

        private static bool IsInFolder(string relativePath, string dir)
        {
            var directoryName = Path.GetDirectoryName(relativePath);
            if (string.IsNullOrEmpty(directoryName))
            {
                return false;
            }
            return directoryName
                    .Split(Path.DirectorySeparatorChar)
                    .Contains(dir, StringComparer.InvariantCultureIgnoreCase);
        }

        public void Backup()
        {
            var diff = GetDiffFiles();
            BackupBy(diff, _targetFolder, x => x.目標路徑, CompareState.刪除檔案, CompareState.時間不同);
            BackupBy(diff, _sourceFolder, x => x.來源路徑, CompareState.新增檔案, CompareState.時間不同);
            Overwrite(diff);
        }

        private static void BackupBy(IEnumerable<FileDTO> sourceDTOs, string folder, Func<FileDTO, string> func, params CompareState[] states)
        {
            var backupFolder = BuildBackupFolder(folder);
            var backupSource = sourceDTOs
                .Where(x => states.Contains(x.狀態))
                .ToList();
            Copy(backupSource, backupFolder, func);
        }

        private void Overwrite(IEnumerable<FileDTO> diffDTOs)
        {
            var copyDTOs = diffDTOs
                .Where(x => x.狀態 == CompareState.新增檔案 || x.狀態 == CompareState.時間不同)
                .ToList();
            Copy(copyDTOs, _targetFolder, x => x.來源路徑);

            var deleteDTOs = diffDTOs.Where(x => x.狀態 == CompareState.刪除檔案).ToList();
            Delete(deleteDTOs, x => x.目標路徑);
        }

        private static string BuildBackupFolder(string sourceFolder)
        {
            var host = BuildBackupHost(sourceFolder);
            var time = DateTime.Now.ToString(Format);
            return Path.Combine(host, time);
        }

        private static string BuildBackupHost(string sourceFolder)
        {
            var host = Directory.GetCurrentDirectory();
            var filename = Path.GetFileName(sourceFolder);
            var result = Path.Combine(host, "Backup", filename);
            CreateFolder(result);
            return result;
        }

        private static void CreateFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        private static void Copy(IEnumerable<FileDTO> dtos, string folder, Func<FileDTO, string> func)
        {
            foreach (var dto in dtos)
            {
                var sourcePath = func.Invoke(dto);
                var targetPath = Path.Combine(folder, dto.相對路徑);
                var targetFolder = Path.GetDirectoryName(targetPath);
                CreateFolder(targetFolder);
                File.Copy(sourcePath, targetPath, true);
            }
        }

        private static void Delete(IEnumerable<FileDTO> dtos, Func<FileDTO, string> func)
        {
            foreach (var dto in dtos)
            {
                var path = func.Invoke(dto);
                File.Delete(path);
            }
        }

        public List<FolderDTO> GetBackupFolders()
        {
            return GetBackupFolders(_targetFolder);
        }

        private static List<FolderDTO> GetBackupFolders(string sourceFolder)
        {
            var folder = BuildBackupHost(sourceFolder);
            return Directory
                .EnumerateDirectories(folder, "*", SearchOption.TopDirectoryOnly)
                .Where(x => DateTime.TryParseExact(Path.GetFileName(x), Format, null, DateTimeStyles.None, out _))
                .Select(x => new FolderDTO(x))
                .ToList();
        }

        public void Restore(string backupFolder)
        {
            Restore(backupFolder, _targetFolder);
        }

        private static void Restore(string backupFolder, string targetFolder)
        {
            var dtos = GetDictionary(backupFolder)
                .Select(x => new FileDTO(x.Key, new FileInfo(x.Value), null))
                .ToList();
            Copy(dtos, targetFolder, x => x.來源路徑);
        }
    }
}
