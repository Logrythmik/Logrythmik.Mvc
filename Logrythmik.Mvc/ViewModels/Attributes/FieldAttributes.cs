using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Logrythmik.Mvc.ViewModels.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FieldAttributes : Attribute, IMetadataAware
    {
        public int MaxLength { get; set; }
        public bool ReadOnly { get; set; }
        public bool Disabled { get; set; }
        public string AccessKey { get; set; }
        public int TabIndex { get; set; }
        public string CssClass { get; set; }


        /// <summary>
        /// Use this field to add custom attributes.
        /// Format: property, value | property, value
        /// </summary>
        /// <value>The other.</value>
        public string Other { get; set; }
        
        public Dictionary<string, object> OptionalAttributes()
        {   
            var options = new Dictionary<string, object>();

            if (MaxLength != 0)
                options.Add("maxlength", MaxLength);

            if (ReadOnly)
                options.Add("readonly", "readonly");

            if (Disabled)
                options.Add("disabled", "disabled");

            if (!string.IsNullOrWhiteSpace(AccessKey))
                options.Add("accessKey", AccessKey);

            if (TabIndex != 0)
                options.Add("tabindex", TabIndex);

            if (!string.IsNullOrWhiteSpace(CssClass))
                options.Add("class", CssClass);

            if (!string.IsNullOrWhiteSpace(Other))
            {
                var valuePairs = Other.Split('|');
                foreach (var valuePair in valuePairs)
                {
                    var segments = valuePair.Split(',');
                    if(segments.Length > 1)
                        options.Add(segments[0], segments[1]);
                }
            }

            return options;
        }

        public const string FIELD_CONSTANT = "FieldAttributes";

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues.Add(FIELD_CONSTANT, OptionalAttributes());
        }
    }
}