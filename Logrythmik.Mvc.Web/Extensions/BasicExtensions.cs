using System;
using System.Text;
using System.Web;
using System.Web.WebPages;

namespace Logrythmik.Mvc
{
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