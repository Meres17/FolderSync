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

        public async Task ApplyAsync(IReadOnlyList<FileStatus> changes, string sourceDir, string destDir, IProgress<SyncProgress>? progress = null)
        {
            if (changes.Count == 0)
            {
                throw new DomainException("沒有需要更新的檔案");
            }

            var backupDir = _backups.CreateBackupDirectory(sourceDir, destDir);
            await BackupChangesAsync(changes, backupDir, sourceDir, destDir, progress);

            try
            {
                await ApplyToDestinationAsync(changes, destDir, progress);
            }
            catch (Exception applyEx)
            {
                // 套用中途失敗時，用剛建立的備份把目標還原回更新前
                try
                {
                    await RollbackAsync(backupDir, destDir, progress);
                }
                catch (Exception rollbackEx)
                {
                    // 還原也失敗：保留兩個例外，避免原始失敗原因被覆蓋
                    throw new AggregateException("套用失敗後自動還原也失敗", applyEx, rollbackEx);
                }
                throw;
            }
        }

        private Task BackupChangesAsync(IReadOnlyList<FileStatus> changes, string backupDir, string sourceDir, string destDir, IProgress<SyncProgress>? progress)
        {
            var rules = BackupPlan.ForOverwrite(Path.GetFileName(sourceDir), Path.GetFileName(destDir));
            var counter = new ProgressCounter("備份", rules.Sum(rule => changes.Count(rule.Matches)), progress);
            var tasks = rules.Select(rule => _files.CopyAsync(
                changes.Where(rule.Matches).Select(file => new CopyOperation(file.PathOf(rule.Side), file.相對路徑)),
                Path.Combine(backupDir, rule.FolderName),
                counter.Increment));
            return Task.WhenAll(tasks);
        }

        private Task ApplyToDestinationAsync(IReadOnlyList<FileStatus> changes, string destDir, IProgress<SyncProgress>? progress)
        {
            var copies = changes
                .Where(x => x.狀態 is CompareState.新增檔案 or CompareState.時間不同)
                .Select(file => new CopyOperation(file.PathOf(FileSide.來源), file.相對路徑))
                .ToList();
            var deletes = changes
                .Where(x => x.狀態 == CompareState.刪除檔案)
                .Select(x => x.目標路徑)
                .ToList();
            var counter = new ProgressCounter("更新", copies.Count + deletes.Count, progress);
            var copy = _files.CopyAsync(copies, destDir, counter.Increment);
            var delete = _files.DeleteAsync(deletes, counter.Increment);
            return Task.WhenAll(copy, delete);
        }

        public async Task RollbackAsync(string backupDir, string destDir, IProgress<SyncProgress>? progress = null)
        {
            //反向操作：還原被刪除與被修改的檔案，移除新增的檔案
            progress?.Report(new SyncProgress("讀取備份中…", 0, 0));
            var deleted = _backups.ReadBackup(backupDir, BackupCategory.Delete);
            var diff = _backups.ReadBackup(backupDir, BackupCategory.Diff);
            var added = _backups.ReadBackup(backupDir, BackupCategory.Add);

            var counter = new ProgressCounter("還原", deleted.Count + diff.Count + added.Count, progress);
            var restoreDeleted = CopyBackupAsync(deleted, destDir, counter.Increment);
            var restoreDiff = CopyBackupAsync(diff, destDir, counter.Increment);
            var removeAdded = _files.DeleteAsync(added.Select(x => Path.Combine(destDir, x.相對路徑)), counter.Increment);
            await Task.WhenAll(restoreDeleted, restoreDiff, removeAdded);
        }

        private Task CopyBackupAsync(IReadOnlyList<FileSnapshot> files, string destHost, Action onFileDone)
        {
            return _files.CopyAsync(files.Select(file => new CopyOperation(file.完整路徑, file.相對路徑)), destHost, onFileDone);
        }
    }
}
