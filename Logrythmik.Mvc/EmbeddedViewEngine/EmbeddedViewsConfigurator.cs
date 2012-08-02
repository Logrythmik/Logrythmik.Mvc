using System.Web.Hosting;

namespace Logrythmik.Mvc.Views
{
    public static class EmbeddedViewsConfigurator
    {
        private static bool _IsConfigured;

        public static void Setup()
        {
            if (_IsConfigured) return;

            var virtualPathProvider = HostingEnvironment.VirtualPathProvider;
            var table = new EmbeddedViewResolver().GetEmbeddedViews();
            var embeddedProvider = new EmbeddedViewVirtualPathProvider(table);
            embeddedProvider.SetDefaultVirtualPathProvider(virtualPathProvider);
            HostingEnvironment.RegisterVirtualPathProvider(embeddedProvider);

            _IsConfigured = true;
        }
    }
}