namespace FolderSync.Infrastructure.Synchronizers
{
    internal class FolderSynchronizer : IFolderSynchronizer
    {
        private readonly IBackupRepository _backups;
        private readonly IFileOperations _files;

        public FolderSynchronizer(IBackupRepository backups, IFileOperations files)
        {
            _backups = backups ?? throw new ArgumentNullException(nameof(backups));
            _files = files ?? throw new ArgumentNullException(nameof(files));
        }

        public async Task ApplyAsync(IReadOnlyList<FileStatus> changes, string sourceDir, string destDir)
        {
            if (changes.Count == 0)
            {
                throw new DomainException("沒有需要更新的檔案");
            }

            var backupDir = _backups.CreateBackupDirectory(sourceDir, destDir);
            await BackupChangesAsync(changes, backupDir, sourceDir, destDir);

            try
            {
                await ApplyToDestinationAsync(changes, destDir);
            }
            catch
            {
                // 套用中途失敗時，用剛建立的備份把目標還原回更新前
                await RollbackAsync(backupDir, destDir);
                throw;
            }
        }

        private Task BackupChangesAsync(IReadOnlyList<FileStatus> changes, string backupDir, string sourceDir, string destDir)
        {
            var rules = BackupPlan.ForOverwrite(Path.GetFileName(sourceDir), Path.GetFileName(destDir));
            var tasks = rules.Select(rule => _files.CopyAsync(
                changes.Where(rule.Matches).Select(file => new CopyOperation(file.PathOf(rule.Side), file.相對路徑)),
                Path.Combine(backupDir, rule.FolderName)));
            return Task.WhenAll(tasks);
        }

        private Task ApplyToDestinationAsync(IReadOnlyList<FileStatus> changes, string destDir)
        {
            var copy = _files.CopyAsync(
                changes.Where(x => x.狀態 is CompareState.新增檔案 or CompareState.時間不同)
                       .Select(file => new CopyOperation(file.PathOf(FileSide.來源), file.相對路徑)),
                destDir);
            var delete = _files.DeleteAsync(changes
                .Where(x => x.狀態 == CompareState.刪除檔案)
                .Select(x => x.目標路徑));
            return Task.WhenAll(copy, delete);
        }

        public async Task RollbackAsync(string backupDir, string destDir)
        {
            //反向操作：還原被刪除與被修改的檔案，移除新增的檔案
            var restoreDeleted = CopyBackupAsync(_backups.ReadBackup(backupDir, BackupCategory.Delete), destDir);
            var restoreDiff = CopyBackupAsync(_backups.ReadBackup(backupDir, BackupCategory.Diff), destDir);
            var removeAdded = _files.DeleteAsync(_backups.ReadBackup(backupDir, BackupCategory.Add)
                .Select(x => Path.Combine(destDir, x.相對路徑)));
            await Task.WhenAll(restoreDeleted, restoreDiff, removeAdded);
        }

        private Task CopyBackupAsync(IReadOnlyList<FileSnapshot> files, string destHost)
        {
            return _files.CopyAsync(files.Select(file => new CopyOperation(file.完整路徑, file.相對路徑)), destHost);
        }
    }
}
