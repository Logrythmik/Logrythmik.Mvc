using System;
using System.Collections;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Logrythmik.Mvc.Views
{

    public class EmbeddedViewVirtualPathProvider : VirtualPathProvider
    {
        private readonly EmbeddedViewTable embeddedViews;
        private VirtualPathProvider defaultProvider;

        public EmbeddedViewVirtualPathProvider(EmbeddedViewTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table", "EmbeddedViewTable cannot be null.");
            }

            embeddedViews = table;
        }

        public void SetDefaultVirtualPathProvider(VirtualPathProvider provider)
        {
            defaultProvider = provider;
        }

        private bool IsEmbeddedView(string virtualPath)
        {
            string checkPath = VirtualPathUtility.ToAppRelative(virtualPath);

            return checkPath.StartsWith("~/Views/", StringComparison.InvariantCultureIgnoreCase)
                   && embeddedViews.ContainsEmbeddedView(checkPath);
        }

        public override bool FileExists(string virtualPath)
        {
            return (IsEmbeddedView(virtualPath) ||
                    defaultProvider.FileExists(virtualPath));
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (!defaultProvider.FileExists(virtualPath) && IsEmbeddedView(virtualPath))
            {
                EmbeddedView embeddedView = embeddedViews.FindEmbeddedView(virtualPath);
                return new AssemblyResourceFile(embeddedView, virtualPath);
            }

            return defaultProvider.GetFile(virtualPath);
        }

        public override CacheDependency GetCacheDependency(
            string virtualPath,
            IEnumerable virtualPathDependencies,
            DateTime utcStart)
        {
            return IsEmbeddedView(virtualPath)
                       ? null
                       : defaultProvider.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }
    }
}