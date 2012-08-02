namespace Logrythmik.Mvc.Proxies
{
    public interface IIoProxy
    {
        void CreateDirectory(string relativePath, string directoryName);
        bool DirectoryExists(string relativePath, string directoryName);

        bool FileExists(string relativePath);
    }
}
