using System;
using System.Web.Mvc;

namespace Logrythmik.Mvc.ViewModels.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DisplayOrderAttribute : Attribute, IMetadataAware
    {
        public int DisplayOrder { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayOrderAttribute"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        public DisplayOrderAttribute(int order)
        {
            this.DisplayOrder = order;
        }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.Order = DisplayOrder;
        }
    }
}
