using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;
using Logrythmik.Mvc.Utilities;
using Logrythmik.Mvc.ViewModels;

namespace Logrythmik.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    public static class HtmlExtensions
    {

        public static string GetCurrentActionUrl(this WebViewPage page)
        {
            var url = new UrlHelper(page.ViewContext.RequestContext);
            return url.Action(page.ViewContext.RouteData.GetRequiredString("action"));
        }

        public static string GetCurrentActionName(this WebViewPage page)
        {
            return page.ViewContext.RouteData.GetRequiredString("action");
        }

        #region UI Helpers

        /// <summary>
        /// Determines whether the specified helper is checked.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="checkValue">if set to <c>true</c> [check value].</param>
        /// <returns></returns>
        public static string IsChecked(this HtmlHelper helper, bool checkValue)
        {
            return checkValue ? "checked=\"checked\"" : "";
        }

        public static string IsDisabled(this HtmlHelper helper, bool disabled)
        {
            return disabled ? "disabled=\"disabled\"" : "";
        }

        #endregion

        public static string AssemblyVersion(this HtmlHelper helper)
        {
            const string key = "AppVersion";
            if (helper.ViewContext.HttpContext.Application[key] == null)
                helper.ViewContext.HttpContext.Application[key] = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return helper.ViewContext.HttpContext.Application[key] as string;
        }

        /// <summary>
        /// Gets the current action URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        public static string GetCurrentActionUrl<T>(HtmlHelper<T> html)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);
            return url.Action(html.ViewContext.RouteData.GetRequiredString("action"));
        }

        public static MvcHtmlString DisplayMessages(this HtmlHelper helper, string messagePartial = "~/Views/Shared/Message.cshtml")
        {
            var outputString = new StringBuilder();

            // Validation Errors
            if (!helper.ViewData.ModelState.IsValid)
            {
                if (helper.ViewData.ModelState.ContainsKey("_FORM"))
                {
                    foreach (var error in helper.ViewData.ModelState["_FORM"].Errors)
                        outputString.Append(helper.Partial(messagePartial,
                        error.Exception != null
                                        ? new ViewMessage(error.Exception)
                                        : new ViewMessage(error.ErrorMessage, MessageType.Error)));
                }
                else
                {
                    var list = new TagBuilder("ul");
                    foreach (var key in helper.ViewData.ModelState.Keys)
                    {
                        foreach (var error in helper.ViewData.ModelState[key].Errors)
                        {
                            var errorItem = new TagBuilder("li");
                            var message = error.ErrorMessage;
                            if (error.Exception != null)
                                message = error.Exception.Message;
                            errorItem.SetInnerText(message);
                            list.InnerHtml += errorItem.ToString();
                        }
                    }

                    outputString.Append(helper.Partial(messagePartial,
                        new ViewMessage("Errors were encountered on submission:",
                            MessageType.Error,
                            list.ToString()
                            )));
                }
            }

            var messages = new List<ViewMessage>();

            // TempData Messages
            var tempMessages = helper.ViewContext.Controller.TempData[Constants.Messages] as IEnumerable<ViewMessage>;
            if (tempMessages != null)
                messages.AddRange(tempMessages);

            // ViewData messages
            var viewDataMessages = helper.ViewData[Constants.Messages] as IEnumerable<ViewMessage>;
            if (viewDataMessages != null)
                messages.AddRange(viewDataMessages);


            // Order and display
            foreach (var message in messages.OrderBy(m => m.Type))
                outputString.Append(helper.Partial(messagePartial, message));

            return new MvcHtmlString(outputString.ToString());
        }

        public static string SanitizeHtml(this string value)
        {
            return HtmlUtilities.Sanitize(value);
        }

        public static string StripTags(this string value)
        {
            return HtmlUtilities.StripTagsRegex(value);
        }

        public static HelperResult List<T>(this IEnumerable<T> items,
            Func<T, HelperResult> template)
        {
            return new HelperResult(writer =>
            {
                foreach (var item in items)
                {
                    template(item).WriteTo(writer);
                }
            });
        }

        public static HelperResult ListWithIndex<TItem>(
            this IEnumerable<TItem> items,
            Func<IndexedItem<TItem>, HelperResult> template)
        {
            return new HelperResult(writer =>
            {
                for (int i = 0; i < items.Count(); i++)
                {
                    var result = template(new IndexedItem<TItem>(i, items.ElementAt(i)));
                    result.WriteTo(writer);
                }
            });
        }

        public static MvcHtmlString Script(this HtmlHelper helper, string url)
        {
            return MvcHtmlString.Create("<script type=\"text/javascript\" src=\"" + url + "\"></script>");
        }

        public static MvcHtmlString CSS(this HtmlHelper helper, string url)
        {
            return MvcHtmlString.Create("<link href=\"" + url + "\" rel=\"stylesheet\" type=\"text/css\" />");
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string url, string alt, string cssClass = null, DateTime? cacheDateStamp = null)
        {
            var img = new TagBuilder("img");
            if (cacheDateStamp.HasValue)
            {
                var dilimeter = url.Contains('?') ? "&" : "?";
                url = url + dilimeter + cacheDateStamp.Value.Ticks;
            }

            img.MergeAttribute("src", url);
            img.MergeAttribute("alt", alt);

            if (!string.IsNullOrWhiteSpace(cssClass))
            {
                img.AddCssClass(cssClass);
            }

            return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing));
        }


        public static string ClipAt(this string value, int length)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length >= length ?
                value.Substring(0, length) + "…" :
                value;
        }
        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        public static void AddMessage(this HtmlHelper helper, MessageType type, string message)
        {
            helper.RenderPartial(Constants.Message, new ViewMessage(message, type));
        }

        /// <summary>
        /// Creates a string with a separator you choose.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="list">The list.</param>
        /// <param name="separator">The separator.</param>
        /// <returns></returns>
        public static string StringRepeater(this HtmlHelper helper, IEnumerable<string> list, string separator)
        {
            return string.Join(separator, list.ToArray());
        }

    }
}
