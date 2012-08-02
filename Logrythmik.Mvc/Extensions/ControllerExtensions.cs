using System;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.WebPages;

namespace Logrythmik.Mvc
{
    public static class ControllerExtensions
    {
        #region Excel Results
        
        public static ExcelResult Excel(this Controller controller,
            IQueryable rows,
            string fileName)
        {
            return new ExcelResult(rows, fileName, null, null, null, null);
        }

        public static ExcelResult Excel(this Controller controller,
            IQueryable rows,
            string fileName,
            string[] headers)
        {
            return new ExcelResult(rows, fileName, headers, null, null, null);
        }

        public static ExcelResult Excel(this Controller controller,
            IQueryable rows,
            string fileName,
            string[] headers,
            TableStyle tableStyle,
            TableItemStyle headerStyle,
            TableItemStyle itemStyle)
        {
            return new ExcelResult(rows, fileName, headers, tableStyle, headerStyle, itemStyle);
        }

        #endregion

        public static ViewResultBase WithData<TDataType>(this ViewResultBase viewResultBase, TDataType data)
        {
            if (Equals(data, default(TDataType)))
                return viewResultBase;
            if (viewResultBase.ViewData.ContainsKey(typeof(TDataType).ToString()))
                viewResultBase.ViewData[typeof(TDataType).ToString()] = data;
            else
                viewResultBase.ViewData.Add(typeof(TDataType).ToString(), data);
            return viewResultBase;
        }

        public static ViewResultBase WithData(this ViewResultBase viewResultBase, Type entityType, object data)
        {
            if (viewResultBase.ViewData.ContainsKey(entityType.ToString()))
                viewResultBase.ViewData[entityType.ToString()] = data;
            else
                viewResultBase.ViewData.Add(entityType.ToString(), data);
            return viewResultBase;
        }        



    }

    public static class BasicExtensions
    {
        public static bool IsNullable(this Type type)
        {
            if (!type.IsValueType)
            {
                return true;
            }

            return (type.IsGenericType && !type.IsGenericTypeDefinition && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        public static string TryToString(this object value)
        {
            return value == null ? string.Empty : value.ToString();
        }

        public static T GetFromRequest<T>(this WebPageBase page, string key)
             where T : IComparable
        {
            if (page.Request[key] != null)
                return (T)Convert.ChangeType(page.Request[key], typeof(T));
            return default(T);
        }

        public static T GetFromSession<T>(this HttpSessionStateBase session, string key, Func<T> functionToSet)
        {
            if (session[key] != null)
                return (T)session[key];

            return (T)(session[key] = functionToSet.Invoke());
        }

        public static string HtmlCharacterEncode(this string text)
        {
            var chars = HttpUtility.HtmlEncode(text).ToCharArray();
            var result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

            foreach (var c in chars)
            {
                var value = Convert.ToInt32(c);
                if (value > 127)
                    result.AppendFormat("&#{0};", value);
                else
                    result.Append(c);
            }

            return result.ToString();
        }    

    }
}