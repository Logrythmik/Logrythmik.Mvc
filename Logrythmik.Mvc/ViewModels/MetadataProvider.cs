using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Logrythmik.Mvc.Utilities;

namespace Logrythmik.Mvc.ViewModels
{
    public class MetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes,
                                                        Type containerType, Func<object> modelAccessor, 
                                                        Type modelType, string propertyName)
        {
            var metadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            // This is a property
            if (propertyName != null && containerType != null) 
            {
                if (modelType.IsEnum && !metadata.HideSurroundingHtml)
                {
                    metadata.TemplateHint = "SelectList";
                    if (metadata.Model != null)
                    {
                        var selectedValue = (int) metadata.Model;
                        metadata.Model = EnumHelper.ToSelectList(modelType, selectedValue);
                    }
                }
            }
           
            var displayName = metadata.GetDisplayName() ?? metadata.PropertyName;

            if (!string.IsNullOrEmpty(displayName))
                metadata.DisplayName = displayName.SplitCamelCase().TrimEnd("View", string.Empty);
            
            return metadata;
        }
    }
}