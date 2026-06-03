namespace FolderSync.Infrastructure.Synchronizers
{
    /// <summary>
    /// 以實際檔案系統執行複製/刪除的 adapter；每個檔案並行處理，來源為 null 則略過。
    /// </summary>
    public class FileOps : IFileOps
    {
        public Task CopyAsync(IEnumerable<CopyOperation> operations, string destinationRoot)
        {
            var tasks = operations.Select(op => Task.Run(() =>
            {
                if (op.Source is null)
                {
                    return;
                }
                var destPath = Path.Combine(destinationRoot, op.RelativePath);
                EnsureDirectory(Path.GetDirectoryName(destPath));
                File.Copy(op.Source, destPath, true);
            }));
            return Task.WhenAll(tasks);
        }

        public Task DeleteAsync(IEnumerable<string?> paths)
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
