using System;
using System.IO;
using System.Web;

namespace Logrythmik.Mvc.Proxies
{
    public class IoProxy : IIoProxy
    {
        public void CreateDirectory(string relativePath, string directoryName)
        {
            var path = HttpContext.Current.Server.MapPath(relativePath.TrimEnd(new[] { '/', '\\' })) + @"\";

            Directory.CreateDirectory(path + directoryName);
        }

        public bool DirectoryExists(string relativePath, string directoryName)
        {
            var path = HttpContext.Current.Server.MapPath(relativePath.TrimEnd(new[] { '/', '\\' })) + @"\";

            return Directory.Exists(path + directoryName);
        }

        public bool FileExists(string relativePath)
        {
            var path = HttpContext.Current.Server.MapPath(relativePath);

            return File.Exists(path);
        }
    }
}