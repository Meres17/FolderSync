namespace FolderSync.Domain.Exceptions
{
    /// <summary>
    /// 違反領域規則時拋出，讓上層能與一般技術例外區分。
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }
    }
}
