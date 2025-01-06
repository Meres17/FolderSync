﻿
using System.Globalization;
using System.Text.Json;

namespace FolderSyncCore
{
    public class FolderComparer
    {
        private const string Format = "yyyyMMdd_HHmm";
        private readonly string _sourceFolder;
        private readonly string _targetFolder;
        private static string _appsettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

        private static AppSettings _appSettings = GetAppSettings();
        public static string Source => _appSettings.Source;
        public static string Target => _appSettings.Target;

        private static AppSettings GetAppSettings()
        {
            try
            {
                var text = File.ReadAllText(_appsettingsPath);
                return JsonSerializer.Deserialize<AppSettings>(text) ?? new AppSettings();
            }
            catch (Exception)
            {
                return new AppSettings();
            }
        }

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

        public void Backup()
        {
            var diff = GetDiffFiles();
            Backup(diff, _sourceFolder, _targetFolder);
            Overwrite(diff);
        }

        private static void Backup(IEnumerable<FileDTO> dtos, string sourceFolder, string targetFolder)
        {
            var backupFolder = BuildBackupFolder(sourceFolder, targetFolder);

            var sourceName = Path.GetFileName(sourceFolder);
            var targetName = Path.GetFileName(targetFolder);

            Backup(dtos, backupFolder, sourceName, x => x.來源路徑, CompareState.新增檔案, CompareState.時間不同);
            Backup(dtos, backupFolder, targetName, x => x.目標路徑, CompareState.刪除檔案, CompareState.時間不同);
            Backup(dtos, backupFolder, "State/Delete", x => x.目標路徑, CompareState.刪除檔案);
            Backup(dtos, backupFolder, "State/Diff", x => x.目標路徑, CompareState.時間不同);
            Backup(dtos, backupFolder, "State/Add", x => x.來源路徑, CompareState.新增檔案);
        }

        private static void Backup(IEnumerable<FileDTO> dtos, string backupHost, string backupName, Func<FileDTO, string> func, params CompareState[] states)
        {
            var deleteDTOs = dtos
                .Where(x => states.Contains(x.狀態))
                .ToList();
            var backupFolder = Path.Combine(backupHost, backupName);
            Copy(deleteDTOs, backupFolder, func);
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

        private static string BuildBackupFolder(string sourceFolder, string targetFolder)
        {
            var host = BuildBackupHost(sourceFolder, targetFolder);
            var time = DateTime.Now.ToString(Format);
            return Path.Combine(host, time);
        }

        private static string BuildBackupHost(string sourceFolder, string targetFolder)
        {
            var host = Directory.GetCurrentDirectory();
            var sourceFileName = Path.GetFileName(sourceFolder);
            var targetFileName = Path.GetFileName(targetFolder);
            var result = Path.Combine(host, "Backup", $"{sourceFileName}_{targetFileName}");
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
            return GetBackupFolders(_sourceFolder, _targetFolder);
        }

        private static List<FolderDTO> GetBackupFolders(string sourceFolder, string targetFolder)
        {
            var folder = BuildBackupHost(sourceFolder, targetFolder);
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
            var deleteDTOs = GetByStateDTOs(backupFolder, "State/Delete");
            Copy(deleteDTOs, targetFolder, x => x.來源路徑);

            var diffDTOs = GetByStateDTOs(backupFolder, "State/Diff");
            Copy(diffDTOs, targetFolder, x => x.來源路徑);

            var addDTOs = GetByStateDTOs(backupFolder, "State/Add");
            var targetDTOs = addDTOs
                .Select(x => new FileDTO(x.相對路徑, new FileInfo(Path.Combine(targetFolder, x.相對路徑)), null))
                .ToList();
            Delete(targetDTOs, x => x.來源路徑);
        }

        private static List<FileDTO> GetByStateDTOs(string backupFolder, string stateName)
        {
            var stateFolder = Path.Combine(backupFolder, stateName);
            return GetDictionary(stateFolder)
                .Select(x => new FileDTO(x.Key, new FileInfo(x.Value), null))
                .ToList();
        }

        public bool ExistOffline()
        {
            return File.Exists(CurrentOfflinePath());
        }

        private string CurrentOfflinePath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "App_offline.htm");
        }

        public void CloseSite()
        {
            File.Copy(CurrentOfflinePath(), TergetOfflinePath(), true);
        }

        private string TergetOfflinePath()
        {
            return Path.Combine(_targetFolder, "App_offline.htm");
        }

        public void OpenSite()
        {
            File.Delete(TergetOfflinePath());
        }
    }
}
