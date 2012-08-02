using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.WebPages;
using Logrythmik.Mvc.ViewModels.Attributes;

namespace Logrythmik.Mvc
{
 
    public static class UrlExtensions
    {
        public static Uri BaseUri(this UrlHelper urlHelper)
        {
            var baseUrl = urlHelper.RequestContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);

            return new Uri(baseUrl);
        }

        public static string GetBaseUrl(this UrlHelper urlHelper)
        {
            if (urlHelper.RequestContext.HttpContext.Request.Url != null)
                return "{1}://{0}".ToFormat(
                    urlHelper.RequestContext.HttpContext.Request.Url.Host, 
                    urlHelper.RequestContext.HttpContext.Request.Url.Scheme);
            return "http://local.sneezebox.com";
        }
    }
    public static class ViewResultExtensions
    {

        public static ActionResult DressViewModel<TViewType>(this ActionResult actionResult, Action<TViewType> dressAction)
            where TViewType: class, new()
        {
            if (actionResult == null) throw new ArgumentNullException("actionResult");

            if (actionResult is ViewResult)
            {
                var viewModel = ((ViewResult) actionResult).Model as TViewType;
                if (viewModel != null)
                    dressAction(viewModel);
            }

            return actionResult;
        }

        public static ViewResult WithPageTitle(this ViewResult viewResult, string pageTitle)
        {
            if (viewResult == null) throw new ArgumentNullException("viewResult");

            viewResult.ViewBag.PageTitle = pageTitle;

            return viewResult;
        }

        public static T GetData<T>(this ViewDataDictionary viewData, string key)
                 where T : IComparable
        {
            if (viewData.ContainsKey(key))
                return (T)Convert.ChangeType(viewData[key], typeof(T));
            return default(T);
        }

        public static T GetData<T>(this ViewDataDictionary viewData)
               where T : IComparable
        {
            if (viewData.ContainsKey(typeof(T).ToString()))
                return (T)Convert.ChangeType(viewData[typeof(T).ToString()], typeof(T));
            return default(T);
        }

        public static T GetData<T>(this WebViewPage page)
        {
            if (page.ViewData.ContainsKey(typeof(T).ToString()))
                return (T)Convert.ChangeType(page.ViewData[typeof(T).ToString()], typeof(T));
            return default(T);
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

        public static MvcHtmlString ScriptBlock(
            this WebViewPage webPage,
            Func<dynamic, HelperResult> template)
        {
            if (!webPage.IsAjax)
            {
                var scriptBuilder = webPage.Context.Items["ScriptBlockBuilder"] as StringBuilder ?? new StringBuilder();

                scriptBuilder.Append(template(null).ToHtmlString());

                webPage.Context.Items["ScriptBlockBuilder"] = scriptBuilder;

                return new MvcHtmlString(string.Empty);
            }
            return new MvcHtmlString(template(null).ToHtmlString());
        }

        public static MvcHtmlString WriteScriptBlocks(this WebViewPage webPage)
        {
            var scriptBuilder = webPage.Context.Items["ScriptBlockBuilder"] as StringBuilder ?? new StringBuilder();

            return new MvcHtmlString(scriptBuilder.ToString());
        }
    }
}
