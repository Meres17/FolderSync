namespace FolderSyncCore
{
    public class FolderComparer
    {
        private readonly string _sourceFolder;
        private readonly string _targetFolder;
        private static AppSettings _appSettings = AppSettings.Build();

        public FolderComparer(string sourceFolder, string targetFolder)
        {
            _sourceFolder = sourceFolder;
            _targetFolder = targetFolder;
        }
        public List<FileDTO> GetDiffFiles()
        {
            return GetDiff(_sourceFolder, _targetFolder, _appSettings.IgnoreFiles, _appSettings.IgnoreFolders);
        }

        private static List<FileDTO> GetDiff(string sourceDic, string targetDic, IEnumerable<string> ignoreFiles, IEnumerable<string> ignoreFolders)
        {

            var sourceDictionary = ComparerHelper.GetPathDictionary(sourceDic, ignoreFiles, ignoreFolders);
            var targetDictionary = ComparerHelper.GetPathDictionary(targetDic, ignoreFiles, ignoreFolders);

            return CompareFiles(sourceDictionary, targetDictionary)
                .Where(x => x.狀態 != CompareState.時間相同)
                .OrderBy(x => x.狀態)
                .ThenBy(x => x.相對路徑)
                .ToList();
        }

        private static List<FileDTO> CompareFiles(Dictionary<string, string> sourceDictionary, Dictionary<string, string> targetDictionary)
        {
            List<FileDTO> result = new();
            foreach (var source in sourceDictionary)
            {
                var sourcePath = source.Key;
                var sourceFullPath = source.Value;
                var sourceFile = new FileInfo(sourceFullPath);

                if (targetDictionary.TryGetValue(sourcePath, out var targetFullPath))
                {
                    var targetFile = new FileInfo(targetFullPath);
                    result.Add(new FileDTO(sourcePath, sourceFile, targetFile));
                    targetDictionary.Remove(sourcePath); // 移除已經比對過的檔案
                }
                else
                {
                    result.Add(new FileDTO(sourcePath, sourceFile, null));
                }
            }

            // 剩下的檔案是 targetFolder 中多出的檔案
            foreach (var target in targetDictionary)
            {
                var targetPath = target.Key;
                var targetFullPath = target.Value;
                var targetFile = new FileInfo(targetFullPath);
                result.Add(new FileDTO(targetPath, null, targetFile));
            }

            return result;
        }

        public void Backup()
        {
            var files = GetDiffFiles();
            BackupAll(files, _sourceFolder, _targetFolder);
            Overwrite(files);
        }

        private static void BackupAll(IEnumerable<FileDTO> dtos, string sourceDir, string targetDir)
        {
            var backupDir = ComparerHelper.CreateBackupDirectory(sourceDir, targetDir);
            var sourceName = Path.GetFileName(sourceDir);
            BackupBy(dtos, backupDir, sourceName, x => x.來源路徑, CompareState.新增檔案, CompareState.時間不同);
            var targetName = Path.GetFileName(targetDir);
            BackupBy(dtos, backupDir, targetName, x => x.目標路徑, CompareState.刪除檔案, CompareState.時間不同);
            BackupBy(dtos, backupDir, "_state/Delete", x => x.目標路徑, CompareState.刪除檔案);
            BackupBy(dtos, backupDir, "_state/Diff", x => x.目標路徑, CompareState.時間不同);
            BackupBy(dtos, backupDir, "_state/Add", x => x.來源路徑, CompareState.新增檔案);
        }

        private static void BackupBy(IEnumerable<FileDTO> dtos, string backupHost, string backupName, Func<FileDTO, string> func, params CompareState[] states)
        {
            var filters = dtos
                .Where(x => states.Contains(x.狀態))
                .ToList();
            var backupDir = Path.Combine(backupHost, backupName);
            ComparerHelper.Copy(filters, backupDir, func);
        }

        private void Overwrite(IEnumerable<FileDTO> dtos)
        {
            var copyDTOs = dtos
                .Where(x => x.狀態 == CompareState.新增檔案 || x.狀態 == CompareState.時間不同)
                .ToList();
            ComparerHelper.Copy(copyDTOs, _targetFolder, x => x.來源路徑);

            var deleteDTOs = dtos.Where(x => x.狀態 == CompareState.刪除檔案).ToList();
            ComparerHelper.Delete(deleteDTOs, x => x.目標路徑);
        }

        public List<FolderDTO> GetBackupFolders()
        {
            return ComparerHelper.GetBackupFolders(_sourceFolder, _targetFolder);
        }

        public void Restore(string backupDir)
        {
            Restore(backupDir, _targetFolder, _appSettings.IgnoreFiles, _appSettings.IgnoreFolders);
        }

        private static void Restore(string backupDir, string targetDir, IEnumerable<string> ignoreFiles, IEnumerable<string> ignoreFolders)
        {
            var deleteDTOs = GetByStateDTOs(backupDir, "_state/Delete", ignoreFiles, ignoreFolders);
            ComparerHelper.Copy(deleteDTOs, targetDir, x => x.來源路徑);

            var diffDTOs = GetByStateDTOs(backupDir, "_state/Diff", ignoreFiles, ignoreFolders);
            ComparerHelper.Copy(diffDTOs, targetDir, x => x.來源路徑);

            var addDTOs = GetByStateDTOs(backupDir, "_state/Add", ignoreFiles, ignoreFolders);
            var targetDTOs = addDTOs
                .Select(x => new FileDTO(x.相對路徑, new FileInfo(Path.Combine(targetDir, x.相對路徑)), null))
                .ToList();
            ComparerHelper.Delete(targetDTOs, x => x.來源路徑);
        }

        public static List<FileDTO> GetByStateDTOs(string backupFolder, string stateName, IEnumerable<string> ignoreFiles, IEnumerable<string> ignoreFolders)
        {
            var stateFolder = Path.Combine(backupFolder, stateName);
            return ComparerHelper.GetPathDictionary(stateFolder, ignoreFiles, ignoreFolders)
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
