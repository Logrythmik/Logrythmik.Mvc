using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace Logrythmik.Mvc.Views
{
    public class EmbeddedWebFormViewsViewEngine : WebFormViewEngine
	{
        public static void RegisterViewEngine()
        {
            EmbeddedViewsConfigurator.Setup();

            ViewEngines.Engines.Add(new EmbeddedWebFormViewsViewEngine());
        }

        // Format is ":ViewCacheEntry:{cacheType}:{prefix}:{name}:{controllerName}:{culture}"
        private const string CacheKeyFormat = ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}";
        private const string CacheKeyPrefixMaster = "Master";
        private const string CacheKeyPrefixPartial = "Partial";
        private const string CacheKeyPrefixView = "View";
        private static readonly string[] EmptyLocations = new string[1];

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");
            if (String.IsNullOrEmpty(viewName))
                throw new ArgumentNullException("viewName");


            string[] viewLocationsSearched;
            string[] masterLocationsSearched;

            var controllerName = controllerContext.RouteData.GetRequiredString("controller");
            var viewPath = GetPath(controllerContext, ViewLocationFormats, "ViewLocationFormats", viewName, controllerName, CacheKeyPrefixView, useCache, out viewLocationsSearched);
            var masterPath = GetPath(controllerContext, MasterLocationFormats, "MasterLocationFormats", masterName, controllerName, CacheKeyPrefixMaster, useCache, out masterLocationsSearched);

            if (String.IsNullOrEmpty(viewPath) || (String.IsNullOrEmpty(masterPath) && !String.IsNullOrEmpty(masterName)))
            {
                return new ViewEngineResult(viewLocationsSearched.Union(masterLocationsSearched));
            }
            return new ViewEngineResult(CreateView(controllerContext, viewPath, masterPath), this);
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");
            if (String.IsNullOrEmpty(partialViewName))
                throw new ArgumentNullException("partialViewName");

            string[] searched;
            var controllerName = controllerContext.RouteData.GetRequiredString("controller");
            var partialPath = GetPath(controllerContext, PartialViewLocationFormats, "PartialViewLocationFormats", partialViewName, controllerName, CacheKeyPrefixPartial, useCache, out searched);

            if (String.IsNullOrEmpty(partialPath))
            {
                return new ViewEngineResult(searched);
            }

            return new ViewEngineResult(CreatePartialView(controllerContext, partialPath), this);
        }


        #region Private Helpers

        private string CreateCacheKey(string prefix, string name, string controllerName, string culture)
        {
            return String.Format(CultureInfo.InvariantCulture, 
                CacheKeyFormat,
                GetType().AssemblyQualifiedName, prefix, name, controllerName, culture);
        }

        private string GetPath(
            ControllerContext controllerContext, 
            IList<string> locations, 
            string locationsPropertyName, 
            string name, 
            string controllerName, 
            string cacheKeyPrefix, 
            bool useCache, 
            out string[] searchedLocations)
        {
            searchedLocations = EmptyLocations;

            if (String.IsNullOrEmpty(name))
                return String.Empty;

            if (locations == null || locations.Count == 0)
                throw new InvalidOperationException("{0} Property cannot be null.".ToFormat(locationsPropertyName));

            var nameRepresentsPath = IsSpecificPath(name);

            var cacheKey = CreateCacheKey(cacheKeyPrefix, name, (nameRepresentsPath) ? String.Empty : controllerName, Thread.CurrentThread.CurrentUICulture.Name);

            if (useCache)
            {
                var result = ViewLocationCache.GetViewLocation(controllerContext.HttpContext, cacheKey);
                if (result != null)
                    return result;
            }

            return (nameRepresentsPath) ?
                GetPathFromSpecificName(controllerContext, name, cacheKey, ref searchedLocations) :
                GetPathFromGeneralName(controllerContext, locations, name, controllerName, cacheKey, ref searchedLocations);
        }

        private string GetPathFromGeneralName(
            ControllerContext controllerContext, 
            IList<string> locations, 
            string name, 
            string controllerName, 
            string cacheKey, 
            ref string[] searchedLocations)
        {
            var result = String.Empty;
            searchedLocations = EmptyLocations;
            var searched = new List<string>();

            var culture = Thread.CurrentThread.CurrentUICulture;

            if (!culture.Name.Equals("en-US"))
            {
                var segments = culture.Name.Split('-');
                var countryCode = segments[1];

                foreach (var location in locations)
                {
                    // Look for a specific language and culture first.
                    var cultureLocation = location.Replace("{0}", "{0}-" + culture.Name);
                    var virtualPath = String.Format(CultureInfo.InvariantCulture, cultureLocation, name, controllerName);

                    if (FileExists(controllerContext, virtualPath))
                    {
                        searchedLocations = EmptyLocations;
                        result = virtualPath;
                        ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, result);
                        break;
                    }
                    searched.Add(virtualPath);
                    
                    // Then look for the country
                    var countryLocation = location.Replace("{0}", "{0}-" + countryCode);
                    virtualPath = String.Format(CultureInfo.InvariantCulture, countryLocation, name, controllerName);

                    if (FileExists(controllerContext, virtualPath))
                    {
                        searchedLocations = EmptyLocations;
                        result = virtualPath;
                        ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, result);
                        break;
                    }
                    searched.Add(virtualPath);
                }
                
            }

            if (string.IsNullOrEmpty(result))
            {
                for (int i = 0; i < locations.Count; i++)
                {
                    var virtualPath = String.Format(CultureInfo.InvariantCulture, locations[i], name, controllerName);
                    
                    if (FileExists(controllerContext, virtualPath))
                    {
                        searchedLocations = EmptyLocations;
                        result = virtualPath;
                        ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, result);
                        break;
                    }

                    searched.Add(virtualPath);
                }
            }


            searchedLocations = searchedLocations.Concat(searched.ToArray()).ToArray();

            return result;
        }

        private string GetPathFromSpecificName(ControllerContext controllerContext, string name, string cacheKey, ref string[] searchedLocations)
        {
            var result = name;

            if (!FileExists(controllerContext, name))
            {
                result = String.Empty;
                searchedLocations = new[] { name };
            }

            ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, result);
            return result;
        }

        private static bool IsSpecificPath(string path)
        {
            var c = path[0];
            return (c == '~' || c == '/');
        }

        #endregion




	}
}