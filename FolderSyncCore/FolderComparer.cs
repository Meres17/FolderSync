
using System.Globalization;

namespace FolderSyncCore
{
    public class FolderComparer
    {
        private const string Format = "yyyyMMdd_HHmmss";
        private static readonly HashSet<CompareState> _backupStates =
            new() { CompareState.刪除檔案, CompareState.時間不同 };
        private static readonly HashSet<CompareState> _copyStates =
            new() { CompareState.新增檔案, CompareState.時間不同 };

        private readonly string _sourceFolder;
        private readonly string _targetFolder;

        public FolderComparer(string sourceFolder, string targetFolder)
        {
            _sourceFolder = sourceFolder;
            _targetFolder = targetFolder;
        }

        public List<FileDTO> GetDiff()
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
                    targetDic.Remove(sourcePath);// 移除已經比對過的檔案
                }
                else
                {
                    result.Add(new FileDTO(sourcePath, sourceFile, null));
                }
            }

            // 剩下的檔案是 path2 中多出的檔案
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
                .ToDictionary(path => Path.GetRelativePath(source, path), path => path);
        }

        public void Backup()
        {
            var diff = GetDiff();
            Backup(diff, _sourceFolder, _targetFolder);
        }

        private static void Backup(IEnumerable<FileDTO> sourceDTOs, string sourceFolder, string targetFolder)
        {
            var backupFolder = BuildBackupFolder(sourceFolder);

            var backoutDTOs = sourceDTOs.Where(x => _backupStates.Contains(x.狀態)).ToList();
            Copy(backoutDTOs, backupFolder, false);

            var copyDTOs = sourceDTOs.Where(x => _copyStates.Contains(x.狀態)).ToList();
            Copy(copyDTOs, targetFolder, true);

            var deleteDTOs = sourceDTOs.Where(x => x.狀態 == CompareState.刪除檔案).ToList();
            Delete(deleteDTOs);
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

        private static void Copy(IEnumerable<FileDTO> dtos, string folder, bool overwrite = false)
        {
            foreach (var dto in dtos)
            {
                var sourcePath = dto.完整路徑;
                var targetPath = Path.Combine(folder, dto.相對路徑);
                var targetFolder = Path.GetDirectoryName(targetPath);
                CreateFolder(targetFolder);
                File.Copy(sourcePath, targetPath, overwrite);
            }
        }

        private static void Delete(IEnumerable<FileDTO> dtos)
        {
            foreach (var dto in dtos)
            {
                File.Delete(dto.完整路徑);
            }
        }

        public List<FolderDTO> GetBackupFolders()
        {
            return GetBackupFolders(_sourceFolder);
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
            Copy(dtos, targetFolder, true);
        }
    }
}
