
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

        public static void Backup(IEnumerable<FileDTO> dtos, string sourceFolder, string targetFolder)
        {
            var backoutDTOs = dtos.Where(x => _backupStates.Contains(x.差異)).ToList();
            var backupFolder = BuilderFolder(sourceFolder);
            CreateFolder(backupFolder);
            Copy(dtos, backupFolder, false);//備份不覆蓋
            Copy(dtos, targetFolder, true);
        }

        private static void Copy(IEnumerable<FileDTO> dtos, string folder, bool overwrite = false)
        {
            var copyDTOs = dtos.Where(x => _copyStates.Contains(x.差異)).ToList();
            foreach (var dto in copyDTOs)
            {
                var sourcePath = dto.完整路徑;
                var targetPath = Path.Combine(folder, dto.相對路徑);
                var targetFolder = Path.GetDirectoryName(targetPath);
                CreateFolder(targetFolder);
                File.Copy(sourcePath, targetPath, overwrite);
            }
        }

        public static void DeleteOnlyTarget(IEnumerable<FileDTO> dtos)
        {
            var deleteDTOs = dtos.Where(x => x.差異 == CompareState.刪除檔案).ToList();
            foreach (var dto in deleteDTOs)
            {
                File.Delete(dto.完整路徑);
            }
        }

        private static void CreateFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        public static List<FileDTO> Diff(string sourceFolder, string targetFolder)
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
                .Where(x => x.差異 != CompareState.時間相同)
                .OrderBy(x => x.差異)
                .ThenBy(x => x.相對路徑)
                .ToList();
        }

        private static Dictionary<string, string> GetDictionary(string source)
        {
            return Directory
                .EnumerateFiles(source, "*", SearchOption.AllDirectories)
                .ToDictionary(path => Path.GetRelativePath(source, path), path => path);
        }

        public static List<FolderDTO> BackupFolders(string sourceFolder)
        {
            var folder = BuildBackupHost(sourceFolder);
            //搜尋格式為 yyyyMMdd_HHmmss 的資料夾

            return Directory
                .EnumerateDirectories(folder, "*", SearchOption.TopDirectoryOnly)
                .Where(x => DateTime.TryParseExact(Path.GetFileName(x), Format, null, DateTimeStyles.None, out _))
                .Select(x => new FolderDTO(x))
                .ToList();
        }

        private static string BuildBackupHost(string sourceFolder)
        {
            var host = Directory.GetCurrentDirectory();
            var filename = Path.GetFileName(sourceFolder);
            return Path.Combine(host, "Backup", filename);
        }

        private static string BuilderFolder(string sourceFolder)
        {
            var host = BuildBackupHost(sourceFolder);
            var time = DateTime.Now.ToString(Format);
            return Path.Combine(host, time);
        }

        public static void Restore(string backupFolder, string targetFolder)
        {
            var dtos = GetDictionary(backupFolder)
                .Select(x => new FileDTO(x.Key, new FileInfo(x.Value), null))
                .ToList();
            Copy(dtos, targetFolder, true);
        }
    }
}
