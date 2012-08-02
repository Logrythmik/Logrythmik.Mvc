using System;
using System.Reflection;

namespace Logrythmik.Mvc.Views
{
    public class EmbeddedViewResolver
    {
        public EmbeddedViewTable GetEmbeddedViews()
        {
            Assembly[] assemblies = GetAssemblies();
            if (assemblies == null || assemblies.Length == 0) return null;

            var table = new EmbeddedViewTable();

            foreach (var assembly in assemblies)
            {
                var names = GetNamesOfAssemblyResources(assembly);
                if (names == null || names.Length == 0) continue;

                foreach (var name in names)
                {
                    var key = name.ToLowerInvariant();
                    if (!key.Contains(".views.")) continue;

                    table.AddView(name, assembly.FullName);
                }
            }

            return table;
        }

        protected virtual Assembly[] GetAssemblies()
        {
            try
            {
                return AppDomain.CurrentDomain.GetAssemblies();
            }
            catch
            {
                return null;
            }
        }

        private static string[] GetNamesOfAssemblyResources(Assembly assembly)
        {
            // GetManifestResourceNames will throw a NotSupportedException when run on a dynamic assembly
            try
            {
                return assembly.GetManifestResourceNames();
            }
            catch
            {
                return new string[] { };
            }
        }
    }
}