using System;

namespace Logrythmik.Mvc.Views
{
    [Serializable]
    public class EmbeddedView
    {
        public string Name { get; set; }
        public string AssemblyFullName { get; set; }
    }
}