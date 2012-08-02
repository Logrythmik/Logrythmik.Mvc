using System;
using System.Collections.Generic;
using System.Linq;

namespace Logrythmik.Mvc.Views
{
    [Serializable]
    public class EmbeddedViewTable
    {
        private static readonly object _lock = new object();
        private readonly Dictionary<string, EmbeddedView> viewCache;

        public EmbeddedViewTable()
        {
            viewCache = new Dictionary<string, EmbeddedView>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void AddView(string viewName, string assemblyName)
        {
            lock (_lock)
            {
                viewCache[viewName] = new EmbeddedView { Name = viewName, AssemblyFullName = assemblyName };
            }
        }

        public IList<EmbeddedView> Views
        {
            get
            {
                return viewCache.Values.ToList();
            }
        }

        public bool ContainsEmbeddedView(string viewPath)
        {
            var foundView = FindEmbeddedView(viewPath);
            return (foundView != null);
        }

        public EmbeddedView FindEmbeddedView(string viewPath)
        {
            var name = GetNameFromPath(viewPath);
            if (string.IsNullOrEmpty(name)) return null;

            return Views.Where(view => view.Name.ToLower().Contains(name.ToLower())).SingleOrDefault();
        }

        protected string GetNameFromPath(string viewPath)
        {
            if (string.IsNullOrEmpty(viewPath)) return null;
            var name = viewPath.Replace("/", ".");
            return name.Replace("~", "");
        }
    }
}