using System.Web;
using System.Web.Routing;

namespace Logrythmik.Mvc.Routing
{
    public class LowercaseRoute : RouteBase
    {
        public LowercaseRoute(RouteBase route)
        {
            _Route = route;
        }

        readonly RouteBase _Route;

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            var path = _Route.GetVirtualPath(requestContext, values);
            if (path != null)
                LowercaseUrlPath(path);

            return path;
        }

        static void LowercaseUrlPath(VirtualPathData pathData)
        {
            var url = pathData.VirtualPath;
            var queryIndex = url.IndexOf('?');
            if (queryIndex < 0) queryIndex = url.Length;

            pathData.VirtualPath =
                url.Substring(0, queryIndex).ToLowerInvariant() +
                url.Substring(queryIndex);
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            return _Route.GetRouteData(httpContext);
        }
    }
}
