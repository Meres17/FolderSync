namespace FolderSync.Infrastructure.Synchronizers
{
    /// <summary>
    /// 以實際檔案系統執行複製/刪除的 adapter；每個檔案並行處理，來源為 null 則略過。
    /// </summary>
    public class FileOperations : IFileOperations
    {
        // 限制同時進行的檔案 IO 數，避免大量檔案時塞爆 thread pool 與磁碟爭用。
        private static readonly ParallelOptions Parallelism = new() { MaxDegreeOfParallelism = Environment.ProcessorCount };

        public Task CopyAsync(IEnumerable<CopyOperation> operations, string destinationRoot)
        {
            return Parallel.ForEachAsync(operations, Parallelism, (op, _) =>
            {
                if (op.Source is not null)
                {
                    var destPath = Path.Combine(destinationRoot, op.RelativePath);
                    EnsureDirectory(Path.GetDirectoryName(destPath));
                    File.Copy(op.Source, destPath, true);
                }
                return ValueTask.CompletedTask;
            });
        }

        public Task DeleteAsync(IEnumerable<string?> paths)
        {
            return Parallel.ForEachAsync(paths, Parallelism, (path, _) =>
            {
                if (path is not null)
                {
                    File.Delete(path);
                }
                return ValueTask.CompletedTask;
            });
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
