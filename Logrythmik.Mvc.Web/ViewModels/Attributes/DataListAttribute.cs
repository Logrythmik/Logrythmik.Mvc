using System;
using System.Web.Mvc;

namespace Logrythmik.Mvc.ViewModels.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DataListAttribute : Attribute, IMetadataAware
    {
        public string ListPropertyName { get; set; }

        public DataListAttribute() {}

        public DataListAttribute(string listPropertyName)
        {
            this.ListPropertyName = listPropertyName;
        }

        public const string DATALIST = "DataList";

        public const string PROPERTYNAME_KEY = "DataList_Property";
        
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.TemplateHint = DATALIST;
            metadata.AdditionalValues.Add(PROPERTYNAME_KEY, this.ListPropertyName ?? metadata.PropertyName + "_List");
        }
    }


}