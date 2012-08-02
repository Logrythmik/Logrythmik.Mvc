using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;

namespace Logrythmik.Mvc.Views
{
    public class AssemblyResourceFile : VirtualFile
    {
        private readonly EmbeddedView _EmbeddedView;

        public AssemblyResourceFile(EmbeddedView view, string virtualPath) :
            base(virtualPath)
        {
            if (view == null) throw new ArgumentNullException("view", "EmbeddedView cannot be null.");

            _EmbeddedView = view;
        }

        public override Stream Open()
        {
            var assembly = GetResourceAssembly();
            return assembly == null ? null : assembly.GetManifestResourceStream(_EmbeddedView.Name);
        }

        protected virtual Assembly GetResourceAssembly()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies
                .Where(assembly =>
                                    string.Equals(assembly.FullName, _EmbeddedView.AssemblyFullName,
                                                  StringComparison.InvariantCultureIgnoreCase))
                .SingleOrDefault();
        }
    }
}