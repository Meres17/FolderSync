
namespace FolderSync.Infrastructure.Synchronizers
{
    internal class FolderSynchronizer : IFolderSynchronizer
    {
        private readonly IBackupRepository _backups;

        public FolderSynchronizer(IBackupRepository backups)
        {
            _backups = backups ?? throw new ArgumentNullException(nameof(backups));
        }

        public async Task ApplyAsync(IReadOnlyList<FileStatus> changes, string sourceDir, string destDir)
        {
            if (changes.Count == 0)
            {
                throw new DomainException("沒有需要更新的檔案");
            }

            var backupDir = _backups.CreateBackupDirectory(sourceDir, destDir);
            await BackupAsync(changes, backupDir, sourceDir, destDir);
            await ApplyToDestAsync(changes, destDir);
        }

        private Task BackupAsync(IReadOnlyList<FileStatus> changes, string backupDir, string sourceDir, string destDir)
        {
            var rules = BackupPlan.ForOverwrite(Path.GetFileName(sourceDir), Path.GetFileName(destDir));
            var tasks = rules.Select(rule => CopyOpsAsync(
                changes.Where(rule.Matches).Select(file => new CopyOperation(file.PathOf(rule.Side), file.相對路徑)),
                Path.Combine(backupDir, rule.Folder)));
            return Task.WhenAll(tasks);
        }

        private Task ApplyToDestAsync(IReadOnlyList<FileStatus> changes, string destDir)
        {
            var copy = CopyOpsAsync(
                changes.Where(x => x.狀態 is CompareState.新增檔案 or CompareState.時間不同)
                       .Select(file => new CopyOperation(file.PathOf(FileSide.來源), file.相對路徑)),
                destDir);
            var delete = DeleteAsync(changes
                .Where(x => x.狀態 == CompareState.刪除檔案)
                .Select(x => x.目標路徑));
            return Task.WhenAll(copy, delete);
        }

        public async Task RollbackAsync(string backupDir, string destDir)
        {
            //反向操作：還原被刪除與被修改的檔案，移除新增的檔案
            var restoreDeleted = CopyBackupAsync(_backups.ReadBackup(backupDir, BackupCategory.Delete), destDir);
            var restoreDiff = CopyBackupAsync(_backups.ReadBackup(backupDir, BackupCategory.Diff), destDir);
            var removeAdded = DeleteAsync(_backups.ReadBackup(backupDir, BackupCategory.Add)
                .Select(x => Path.Combine(destDir, x.相對路徑)));
            await Task.WhenAll(restoreDeleted, restoreDiff, removeAdded);
        }

        private Task CopyBackupAsync(IReadOnlyList<FileSnapshot> files, string destHost)
        {
            return CopyOpsAsync(files.Select(file => new CopyOperation(file.完整路徑, file.相對路徑)), destHost);
        }

        internal virtual Task CopyOpsAsync(IEnumerable<CopyOperation> ops, string destHost)
        {
            var tasks = ops.Select(op => Task.Run(() =>
            {
                if (op.Source is null)
                {
                    return;
                }
                var destPath = Path.Combine(destHost, op.RelativePath);
                EnsureDirectory(Path.GetDirectoryName(destPath));
                File.Copy(op.Source, destPath, true);
            }));
            return Task.WhenAll(tasks);
        }

        internal virtual Task DeleteAsync(IEnumerable<string?> paths)
        {
            var tasks = paths.Select(path => Task.Run(() =>
            {
                if (path is not null)
                {
                    File.Delete(path);
                }
            }));
            return Task.WhenAll(tasks);
        }

        private static void EnsureDirectory(string? dir)
        {
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}
