using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Web.Mvc;

namespace Logrythmik.Mvc.ActionFilters
{
    public class SetCultureAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Set Culture Code
            var cultureCode = GetCurrentCulture(filterContext);

            if (string.IsNullOrEmpty(cultureCode)) return;

            filterContext.HttpContext.Response.Cookies.Add(
                new HttpCookie("Culture", cultureCode)
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddYears(100)
                }
            );

            var culture = new CultureInfo(cultureCode);

            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            }
            catch (Exception)
            {
                // ignore
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }



        private static string GetCurrentCulture(ControllerContext filterContext)
        {
            var configuredCultures = ConfigurationManager.AppSettings["SupportedCultures"] ?? "en-US";

            var cultures = new List<string>(configuredCultures.Split(','));

            if(!cultures.Contains("en-US"))
                cultures.Add("en-US");

            var cookieValue = GetCookieCulture(filterContext, cultures);

            if (string.IsNullOrEmpty(cookieValue))
            {
                var browserCulture = GetBrowserCulture(filterContext, cultures);
                return string.IsNullOrEmpty(browserCulture)
                            ? "en-US"
                            : browserCulture;

            }
            return cookieValue;
        }

        private static string GetCookieCulture(ControllerContext
            filterContext, ICollection<string> cultures)
        {
            /* Get the language in the cookie*/
            var userCookie = filterContext.RequestContext
                                                .HttpContext
                                                .Request
                                                .Cookies["Culture"];

            if (userCookie != null)
            {
                if (!string.IsNullOrEmpty(userCookie.Value))
                    return cultures.Contains(userCookie.Value) ? userCookie.Value : string.Empty;
                return string.Empty;
            }
            return string.Empty;
        }

        private static string GetBrowserCulture(ControllerContext filterContext, IEnumerable<string> cultures)
        {
            /* Gets Languages from Browser */
            var browserLanguages = filterContext.RequestContext
                                                         .HttpContext
                                                         .Request
                                                         .UserLanguages;

            if (browserLanguages != null)
                foreach (var thisBrowserLanguage in browserLanguages)
                {
                    foreach (var thisCultureLanguage in cultures)
                    {
                        if (thisCultureLanguage != thisBrowserLanguage)
                            continue;

                        return thisCultureLanguage;
                    }
                }
            return string.Empty;
        }
    }
}