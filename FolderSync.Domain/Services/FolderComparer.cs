namespace FolderSync.Domain.Services
{
    /// <summary>
    /// 比對來源與目標的檔案快照，產出有差異的檔案狀態清單。
    /// </summary>
    public class FolderComparer
    {
        public List<FileStatus> Compare(
            IEnumerable<FileSnapshot> source,
            IEnumerable<FileSnapshot> dest,
            IgnorePolicy ignore)
        {
            return Diff(source, dest, path => ignore.ShouldIgnore(path) == false);
        }

        /// <summary>
        /// 只比對符合忽略規則的檔案，產出「有差異但不會被同步」的清單。
        /// </summary>
        public List<FileStatus> CompareIgnored(
            IEnumerable<FileSnapshot> source,
            IEnumerable<FileSnapshot> dest,
            IgnorePolicy ignore)
        {
            return Diff(source, dest, ignore.ShouldIgnore);
        }

        private static List<FileStatus> Diff(
            IEnumerable<FileSnapshot> source,
            IEnumerable<FileSnapshot> dest,
            Func<string, bool> include)
        {
            var sourceMap = ToMap(source, include);
            var destMap = ToMap(dest, include);

            var result = new List<FileStatus>();
            foreach (var (relativePath, sourceFile) in sourceMap)
            {
                destMap.Remove(relativePath, out var destFile);
                result.Add(CreateFileStatus(relativePath, sourceFile, destFile));
            }

            // 剩餘的 destMap 即為僅存在於目標的檔案
            foreach (var (relativePath, destFile) in destMap)
            {
                result.Add(CreateFileStatus(relativePath, null, destFile));
            }

            return result
                .Where(x => x.狀態 != CompareState.時間相同)
                .OrderBy(x => x.狀態)
                .ThenBy(x => x.相對路徑)
                .ToList();
        }

        private static Dictionary<string, FileSnapshot> ToMap(IEnumerable<FileSnapshot> snapshots, Func<string, bool> include)
        {
            return snapshots
                .Where(x => include(x.相對路徑))
                .ToDictionary(x => x.相對路徑);
        }

        private static FileStatus CreateFileStatus(string relativePath, FileSnapshot? source, FileSnapshot? dest)
        {
            return new FileStatus(
                relativePath,
                source?.完整路徑, source?.最後修改時間,
                dest?.完整路徑, dest?.最後修改時間);
        }
    }
}
