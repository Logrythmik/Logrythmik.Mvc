using System;
using System.Web.Mvc;

namespace Logrythmik.Mvc.ViewModels.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExampleAttribute: Attribute, IMetadataAware
    {
        public ExampleAttribute(string example)
        {
            Example = example;
        }

        public string Example { get; set; }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.Watermark = this.Example;
        }
    }
}