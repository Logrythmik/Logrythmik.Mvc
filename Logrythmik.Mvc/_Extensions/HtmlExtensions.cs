#region Copyright

/*
 * Copyright (c) 2004-2008 IP Commerce, INC. - All Rights Reserved.
 *
 * This software and documentation is subject to and made
 * available only pursuant to the terms of an executed license
 * agreement, and may be used only in accordance with the terms
 * of said agreement. This document may not, in whole or in part,
 * be copied, photocopied, reproduced, translated, or reduced to
 * any electronic medium or machine-readable form without
 * prior consent, in writing, from IP Commerce, INC.
 *
 * Use, duplication or disclosure by the U.S. Government is subject
 * to restrictions set forth in an executed license agreement
 * and in subparagraph (c)(1) of the Commercial Computer
 * Software-Restricted Rights Clause at FAR 52.227-19; subparagraph
 * (c)(1)(ii) of the Rights in Technical Data and Computer Software
 * clause at DFARS 252.227-7013, subparagraph (d) of the Commercial
 * Computer Software--Licensing clause at NASA FAR supplement
 * 16-52.227-86; or their equivalent.
 *
 * Information in this document is subject to change without notice
 * and does not represent a commitment on the part of IP Commerce.
 *
 */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;
using Ipc.Mvc.Utilities;
using Ipc.Mvc.ViewModels;

namespace Ipc.Mvc
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
        /// Returns the HTML for a basic editor form that will post back to the current action's URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="html">The HTML.</param>
        /// <param name="submitButtonText">Text to show on the form's submit button.</param>
        /// <param name="additionalHtmlNextToSubmit">The additional HTML next to submit.</param>
        /// <returns></returns>
        public static IHtmlString FormForModel<T>(
            this HtmlHelper<T> html, 
            string submitButtonText = "Submit",
            Func<HtmlString> additionalHtmlNextToSubmit = null)
        {
            var action = GetCurrentActionUrl(html);
            var extraHtml = (additionalHtmlNextToSubmit != null) ? additionalHtmlNextToSubmit() : new HtmlString(string.Empty);
            return new HtmlString(
                "<form method=\"post\" action=\"" + action + "\">" +
                html.EditorForModel() +
                "<div class=\"actions\"><button type=\"submit\">" + submitButtonText + "</button>" +
                 extraHtml + "</div></form>"
            );
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
            //// Unhandled Exception
            //if (helper.ViewData.Model == null)
            //{
            //    var exception = helper.ViewData[IpcConstants.Exception] as Exception;
            //    if (exception == null)
            //        return null;

            //    outputString.Append(helper.Partial(messagePartial, new ViewMessage(exception)));
            //    return null;
            //}

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
            var tempMessages = helper.ViewContext.Controller.TempData[IpcConstants.Messages] as IEnumerable<ViewMessage>;
            if (tempMessages != null)
                messages.AddRange(tempMessages);

            // ViewData messages
            var viewDataMessages = helper.ViewData[IpcConstants.Messages] as IEnumerable<ViewMessage>;
            if(viewDataMessages!=null)
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
    }

    public class IndexedItem<TModel>
    {
        public IndexedItem(int index, TModel item)
        {
            Index = index;
            Item = item;
        }

        public int Index { get; private set; }
        public TModel Item { get; private set; }
    }

    public static class SectionExtensions
    {
        private static readonly object _o = new object();

        public static HelperResult RenderSection(
            this WebPageBase page,
            string sectionName,
            Func<object, HelperResult> defaultContent)
        {
            if (page.IsSectionDefined(sectionName))
                return page.RenderSection(sectionName);

            return defaultContent(_o);
        }

        public static HelperResult RedefineSection(
            this WebPageBase page,
            string sectionName)
        {
            return RedefineSection(page, sectionName, defaultContent: null);
        }

        public static HelperResult RedefineSection(
            this WebPageBase page,
            string sectionName,
            Func<object, HelperResult> defaultContent)
        {
            if (page.IsSectionDefined(sectionName))
            {
                page.DefineSection(sectionName, () => page.Write(page.RenderSection(sectionName)));
            }
            else if (defaultContent != null)
            {
                page.DefineSection(sectionName, () => page.Write(defaultContent(_o)));
            }
            return new HelperResult(x => { });
        }
    }
}
