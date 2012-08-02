using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ipc.Mvc.ViewModels.Attributes;

namespace Ipc.Mvc
{
    public static class ViewResultExtensions
    {
        public static ViewResult WithPageTitle(this ViewResult viewResult, string pageTitle)
        {
            if (viewResult == null) throw new ArgumentNullException("viewResult");

            viewResult.ViewBag.PageTitle = pageTitle;

            return viewResult;
        }

        public static IDictionary<string, object> GetAttributeDictionary(this ModelMetadata modelMetadata, string cssClass = null)
        {
            if (modelMetadata == null) throw new ArgumentNullException("modelMetadata");

            var attributeDictionary = modelMetadata.AdditionalValues.ContainsKey(FieldAttributes.FIELD_CONSTANT) ?
                    modelMetadata.AdditionalValues[FieldAttributes.FIELD_CONSTANT] as Dictionary<string, object> 
                    ?? new Dictionary<string, object>()
                    :  new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(cssClass))
            {
                if (attributeDictionary.ContainsKey("class"))
                    attributeDictionary["class"] = attributeDictionary["class"] + " " + cssClass;
                else
                    attributeDictionary.Add("class", cssClass);
            }

            return attributeDictionary;
        }

        public static SelectList GetDataList(this WebViewPage viewPage) 
        {
            if (viewPage == null) 
                throw new ArgumentNullException("viewPage");

            var listPropertyName = viewPage.ViewData.ModelMetadata.AdditionalValues[DataListAttribute.PROPERTYNAME_KEY] as string;

            if(string.IsNullOrEmpty(listPropertyName))
                throw new ArgumentNullException("listPropertyName");

            var container = viewPage.ViewBag.Container;

            if (container == null)
                throw new ArgumentNullException("container");

            if(container.GetType() != viewPage.ViewData.ModelMetadata.ContainerType)
                throw new ApplicationException(
                    string.Format(
                    "Container type mismatch on ViewModel. Expected: {0} when it was actually {1}",
                        viewPage.ViewData.ModelMetadata.ContainerType, 
                        container.GetType()));

            var property = viewPage.ViewData.ModelMetadata.ContainerType.GetProperty(listPropertyName);
            if (property != null)
                return property.GetValue(container, null) as SelectList;

            throw new ApplicationException("ViewModel did not contain a SelectList Property {0}".ToFormat(listPropertyName));
        }

        public static T GetContainer<T>(this WebViewPage viewPage)
        {
            return (T)viewPage.ViewBag.Container;
        }

        public static TAttribute GetModelAttribute<TAttribute>(this ViewDataDictionary viewData, bool inherit = false) where TAttribute : Attribute
        {
            if (viewData == null) throw new ArgumentNullException("viewData");

            var containerType = viewData.ModelMetadata.ContainerType;

            return ((TAttribute[])containerType.GetProperty(viewData.ModelMetadata.PropertyName)
                                                .GetCustomAttributes(typeof(TAttribute), inherit)).FirstOrDefault();
        }
    }
}
