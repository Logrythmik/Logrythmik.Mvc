using System;
using System.Web.Mvc;

namespace Logrythmik.Mvc.ViewModels.Attributes
{
    public class EmptyStringAsNullAttribute : Attribute, IMetadataAware
    {
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.ConvertEmptyStringToNull = true;
        }
    }
}