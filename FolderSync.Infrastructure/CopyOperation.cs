namespace FolderSync.Infrastructure
{
    /// <summary>
    /// 單一複製動作：來源實體路徑與相對路徑（來源為 null 表示略過）。
    /// </summary>
    internal readonly record struct CopyOperation(string? Source, string RelativePath);
}
