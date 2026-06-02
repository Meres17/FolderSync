namespace FolderSync.Infrastructure.Scanning
{
    public class FileSystemDirectoryProbe : IDirectoryProbe
    {
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }
    }
}
