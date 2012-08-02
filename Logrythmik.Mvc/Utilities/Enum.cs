using System.Linq;
using System.Web.Mvc;
using Logrythmik.Data;

namespace Logrythmik.Mvc.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Enum<T> : Logrythmik.Utilities.Enum<T>
        where T: struct 
    {
        public static SelectList ToSelectList()
        {
            var listItems = from pair in GetDictionary()
                            select new SelectListItem
                            {
                                Text = pair.Value.SplitCamelCase(),
                                Value = pair.Value
                            };

            return new SelectList(listItems, "Value", "Text");
        }


        public static SelectList ToSelectList(string selected)
        {
            var listItems = from pair in GetDictionary()
                            select new SelectListItem
                            {
                                Text = pair.Value.SplitCamelCase(),
                                Value = pair.Value
                            };

            return new SelectList(listItems, "Value", "Text", selected);
        }
    }
}
