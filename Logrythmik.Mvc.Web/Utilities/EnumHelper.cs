using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Logrythmik.Mvc.Utilities
{
    public class EnumHelper
    {

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<int> GetValues(Type enumType)
        {
            return Enum.GetValues(enumType).Cast<int>();
        }

        public static IDictionary<int, string> GetDictionary(Type enumType)
        {
            return GetValues(enumType).ToDictionary(
                value => value,
                value => GetDisplayName(enumType, value));
        }

        public static string GetDisplayName(Type enumType, object value)
        {
            return Enum.GetName(enumType, value).SplitCamelCase().Replace("_", " ");
        }

        public static SelectList ToSelectList(Type enumType, string defaultText, int defaultValue)
        {
            var dictionary = GetDictionary(enumType);
            dictionary.Add(defaultValue, defaultText);

            var listItems = from pair in dictionary
                            select new SelectListItem
                                       {
                                           Text = pair.Value,
                                           Value = pair.Key.ToString()
                                       };

            return new SelectList(listItems, "Value", "Text", defaultValue);
        }

        public static SelectList ToSelectList(Type enumType)
        {
            var listItems = from pair in GetDictionary(enumType)
                            select new SelectListItem
                                       {
                                           Text = pair.Value,
                                           Value = pair.Key.ToString()
                                       };

            return new SelectList(listItems, "Value", "Text");
        }

        public static SelectList ToSelectList(Type enumType, int selectedValue)
        {
            var listItems = from pair in GetDictionary(enumType)
                            select new SelectListItem
                                       {
                                           Text = pair.Value,
                                           Value = pair.Key.ToString()
                                       };

            return new SelectList(listItems, "Value", "Text", selectedValue);
        }

        public static SelectList ToSelectList(Type enumType, string selected)
        {
            var listItems = from pair in GetDictionary(enumType)
                            select new SelectListItem
                                       {
                                           Text = pair.Value,
                                           Value = pair.Value
                                       };

            return new SelectList(listItems, "Value", "Text", selected);
        }
    }
}