using System.Globalization;

namespace FolderSyncCore
{
    internal class FileBackup
    {
        private const string Format = "yyyyMMdd_HHmm";
        private const string DeleteName = "_state/Delete";
        private const string DiffName = "_state/Diff";
        private const string AddName = "_state/Add";
        private AppSettings _appSettings;

        public FileBackup(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public List<FolderDTO> GetFolders(string sourceDir, string targetDir)
        {
            var backupHost = CreateBackupHost(sourceDir, targetDir);
            return Directory
                .EnumerateDirectories(backupHost, "*", SearchOption.TopDirectoryOnly)
                .Where(x => DateTime.TryParseExact(Path.GetFileName(x), Format, null, DateTimeStyles.None, out _))
                .Select(x => new FolderDTO(x))
                .ToList();
        }

        private static string CreateBackupHost(string sourceDir, string targetDir)
        {
            var host = Directory.GetCurrentDirectory();
            var sourceFileName = Path.GetFileName(sourceDir);
            var targetFileName = Path.GetFileName(targetDir);
            var result = Path.Combine(host, "Backup", $"{sourceFileName}_{targetFileName}");
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


        public void Backup(IEnumerable<FileStatus> files, string sourceDir, string targetDir)
        {
            var backupDir = CreateBackupDirectory(sourceDir, targetDir);
            Backup(files, backupDir, Path.GetFileName(sourceDir), x => x.來源路徑, CompareState.新增檔案, CompareState.時間不同);
            Backup(files, backupDir, Path.GetFileName(targetDir), x => x.目標路徑, CompareState.刪除檔案, CompareState.時間不同);
            Backup(files, backupDir, DeleteName, x => x.目標路徑, CompareState.刪除檔案);
            Backup(files, backupDir, DiffName, x => x.目標路徑, CompareState.時間不同);
            Backup(files, backupDir, AddName, x => x.來源路徑, CompareState.新增檔案);

            Overwrite(files, targetDir);
        }

        private static string CreateBackupDirectory(string sourceDir, string targetDir)
        {
            var host = CreateBackupHost(sourceDir, targetDir);
            var time = DateTime.Now.ToString(Format);
            return Path.Combine(host, time);
        }



        private static void Backup(IEnumerable<FileStatus> files, string host, string name, Func<FileStatus, string> func, params CompareState[] states)
        {
            var filters = files
                .Where(x => states.Contains(x.狀態))
                .ToList();
            var backupDir = Path.Combine(host, name);
            Copy(filters, backupDir, func);
        }


        private void Overwrite(IEnumerable<FileStatus> files, string targetDir)
        {
            var copyFiles = files
                .Where(x => x.狀態 == CompareState.新增檔案 || x.狀態 == CompareState.時間不同)
                .ToList();
            Copy(copyFiles, targetDir, x => x.來源路徑);

            var deleteFiles = files.Where(x => x.狀態 == CompareState.刪除檔案).ToList();
            Delete(deleteFiles, x => x.目標路徑);
        }

        private static void Copy(IEnumerable<FileStatus> files, string dir, Func<FileStatus, string> func)
        {
            foreach (var file in files)
            {
                var sourcePath = func.Invoke(file);
                var targetPath = Path.Combine(dir, file.相對路徑);
                var targetDir = Path.GetDirectoryName(targetPath);
                CreateDirectory(targetDir);
                File.Copy(sourcePath, targetPath, true);
            }
        }

        private static void Delete(IEnumerable<FileStatus> dtos, Func<FileStatus, string> func)
        {
            foreach (var dto in dtos)
            {
                var path = func.Invoke(dto);
                File.Delete(path);
            }
        }


        public void Restore(string backupHostDic, string targetDir)
        {
            //反向操作
            var deleteFiles = GetRestoreFiles(backupHostDic, DeleteName);
            Copy(deleteFiles, targetDir, x => x.來源路徑);

            var diffFiles = GetRestoreFiles(backupHostDic, DiffName);
            Copy(diffFiles, targetDir, x => x.來源路徑);

            var addFiles = GetRestoreFiles(backupHostDic, AddName)
                .Select(x => new FileStatus(x.相對路徑, new FileInfo(Path.Combine(targetDir, x.相對路徑)), null))
                .ToList();
            Delete(addFiles, x => x.來源路徑);
        }

        private List<FileStatus> GetRestoreFiles(string host, string name)
        {
            var backupDic = Path.Combine(host, name);
            return new DictionaryComparer(_appSettings)
                .GetPathDictionary(backupDic)
                .Select(x => new FileStatus(x.Key, new FileInfo(x.Value), null))
                .ToList();
        }




    }
}
