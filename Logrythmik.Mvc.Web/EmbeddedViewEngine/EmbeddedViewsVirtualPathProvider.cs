using System;
using System.Collections;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Logrythmik.Mvc.Views
{
    public class EmbeddedViewsVirtualPathProvider : VirtualPathProvider
    {
        private readonly EmbeddedViewTable _EmbeddedViews;
        private VirtualPathProvider _DefaultProvider;

        public EmbeddedViewsVirtualPathProvider(EmbeddedViewTable table)
        {
            if (table == null) {
                throw new ArgumentNullException("table", "EmbeddedViewTable cannot be null.");
            }

            _EmbeddedViews = table;
        }

        public void SetDefaultVirtualPathProvider(VirtualPathProvider provider) {
            _DefaultProvider = provider;
        }

        private bool IsEmbeddedView(string virtualPath) {
            
            var checkPath = VirtualPathUtility.ToAppRelative(virtualPath);

            return checkPath.StartsWith("~/Views/", StringComparison.InvariantCultureIgnoreCase) 
                   && _EmbeddedViews.ContainsEmbeddedView(checkPath);
        }

        public override bool FileExists(string virtualPath) {
            return (_DefaultProvider.FileExists(virtualPath) || IsEmbeddedView(virtualPath));
        }

        public override VirtualFile GetFile(string virtualPath) {
            if (!_DefaultProvider.FileExists(virtualPath) && IsEmbeddedView(virtualPath)) {
                var embeddedView = _EmbeddedViews.FindEmbeddedView(virtualPath);
                return new AssemblyResourceFile(embeddedView, virtualPath);
            }

            return _DefaultProvider.GetFile(virtualPath);
        }

        public override CacheDependency GetCacheDependency(
            string virtualPath,
            IEnumerable virtualPathDependencies,
            DateTime utcStart) {
            return IsEmbeddedView(virtualPath)
                       ? null
                       : _DefaultProvider.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }
    }
}