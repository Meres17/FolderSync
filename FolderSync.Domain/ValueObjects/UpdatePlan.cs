namespace FolderSync.Domain.ValueObjects
{
    /// <summary>
    /// 一次掃描得到的更新計畫：要同步的變更，以及符合忽略規則而略過的差異。
    /// </summary>
    public record UpdatePlan(string 來源, string 目標, IReadOnlyList<FileStatus> 變更, IReadOnlyList<FileStatus> 略過);
}
