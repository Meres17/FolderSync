namespace FolderSync.Domain
{
    /// <summary>
    /// 依同步模式建立對應的 <see cref="IFolderSynchronizer"/>。
    /// </summary>
    public interface IFolderSynchronizerFactory
    {
        IReadOnlyList<SyncMode> Modes { get; }
        IFolderSynchronizer Create(SyncMode mode);
    }
}
