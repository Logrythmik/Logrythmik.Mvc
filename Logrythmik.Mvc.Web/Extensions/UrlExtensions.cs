using System;
using System.Web.Mvc;

namespace Logrythmik.Mvc
{
 
    public static class UrlExtensions
    {
        public static Uri BaseUri(this UrlHelper urlHelper)
        {
            if (urlHelper.RequestContext.HttpContext == null || 
                urlHelper.RequestContext.HttpContext.Request == null ||
                urlHelper.RequestContext.HttpContext.Request.Url == null)
                return null;

            var baseUrl = urlHelper.RequestContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);

            return new Uri(baseUrl);
        }

        public static string GetBaseUrl(this UrlHelper urlHelper)
        {
            if (urlHelper.RequestContext.HttpContext.Request.Url != null)
                return "{1}://{0}".ToFormat(
                    urlHelper.RequestContext.HttpContext.Request.Url.Host, 
                    urlHelper.RequestContext.HttpContext.Request.Url.Scheme);
            return "http://localhost";
        }
    }
}
