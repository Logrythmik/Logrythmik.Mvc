using System;
using System.Web.Mvc;

namespace Logrythmik.Mvc.ViewModels.Attributes
{
    public class HelpAttribute : Attribute, IMetadataAware
    {
        public HelpAttribute(string help)
        {
            Help = help;
        }

        public string Help { get; private set; }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.Description = this.Help;
        }
    }
}