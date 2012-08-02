using System;
using System.Collections.Generic;
using System.Linq;

namespace Logrythmik.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Enum<T>
        where T: struct 
    {
        /// <summary>
        /// Parses the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static T Parse(string name)
        {
            return Parse(name, true);
        }

        /// <summary>
        /// Parses the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public static T Parse(string name, bool ignoreCase)
        {
            return (T)Enum.Parse(typeof(T), name, ignoreCase);
        }

        /// <summary>
        /// Tries to parse.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static T? TryParse(string name)
        {
            return TryParse(name, false);
        }

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public static T? TryParse(string name, bool ignoreCase)
        {
            T value;
            if (!string.IsNullOrEmpty(name) && TryParse(name, out value, ignoreCase))
                return value;
            return null;
        }

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool TryParse(string name, out T value)
        {
            return TryParse(name, out value, false);
        }

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public static bool TryParse(string name, out T value, bool ignoreCase)
        {
            try
            {
                value = Parse(name, ignoreCase);
                return true;
            }
            catch (ArgumentException)
            {
                value = default(T);
                return false;
            }
        }

        /// <summary>
        /// Returns the Enum as an Enumerable.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumerable()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object> GetValues()
        {
            return Enum.GetValues(typeof(T)).Cast<object>();
        }

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <returns></returns>
        public static IDictionary<object, string> GetDictionary()
        {
            return GetValues().ToDictionary(value => 
                Convert.ChangeType(value, Enum.GetUnderlyingType(typeof (T))), GetName);
        }
 
        #region Strongly-Typed Enum Extenders

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string Format(object value, string format)
        {
            return Enum.Format(typeof(T), value, format);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetName(object value)
        {
            return Enum.GetName(typeof(T), value);
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetNames()
        {
            return Enum.GetNames(typeof(T));
        }

        /// <summary>
        /// Gets the type of the underlying.
        /// </summary>
        /// <returns></returns>
        public static Type GetUnderlyingType()
        {
            return Enum.GetUnderlyingType(typeof(T));
        }

        /// <summary>
        /// Determines whether the specified value is defined.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is defined; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDefined(object value)
        {
            return Enum.IsDefined(typeof(T), value);
        }

        /// <summary>
        /// Returns the underlying value as the Enum.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ToObject(object value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        /// Returns the underlying value as the Enum.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ToObject(byte value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        /// Returns the underlying value as the Enum.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ToObject(sbyte value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        /// Returns the underlying value as the Enum.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ToObject(int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        /// Returns the underlying value as the Enum.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ToObject(uint value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        /// Returns the underlying value as the Enum.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ToObject(long value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        /// Returns the underlying value as the Enum.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ToObject(ulong value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        /// Returns the underlying value as the Enum.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ToObject(short value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        /// Returns the underlying value as the Enum.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ToObject(ushort value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        #endregion
    }
}
