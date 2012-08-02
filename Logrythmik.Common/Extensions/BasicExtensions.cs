#region Copyright
/*
 * Copyright (c) 2005-2009 Logrythmik Consulting, LLC. - All Rights Reserved.
 *
 * This software and documentation is subject to and made
 * available only pursuant to the terms of an executed license
 * agreement, and may be used only in accordance with the terms
 * of said agreement. This document may not, in whole or in part,
 * be copied, photocopied, reproduced, translated, or reduced to
 * any electronic medium or machine-readable form without
 * prior consent, in writing, from Logrythmik Consulting, LLC.
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
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Logrythmik
{
    public static class BasicExtensions
    {
        #region String Extensions

        public static string ExtractExtension(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var extension = Path.GetExtension(value);
            if (!string.IsNullOrEmpty(extension))
                return extension.Replace(".", string.Empty).ToLower();

            return value;
        }

        /// <summary>
        /// Toes the format.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        internal static string ToFormat(this string value, params string[] values)
        {
            return string.Format(value, values);
        }


        /// <summary>
        /// Splits a string into words, based on camel casing.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string SplitCamelCase(this string source)
        {
            var regex = new Regex
                          (@"([A-Z][a-z]+|[A-Z]+[A-Z]|[A-Z]|[^A-Za-z]+[^A-Za-z])",
                          RegexOptions.RightToLeft);

            return regex.Replace(source, " $1").Replace("_", string.Empty);
        }

        /// <summary>
        /// Formats the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        internal static string ToFormat(this string format, params object[] args)
        {
            try
            {
                return string.Format(format, args);
            }
            catch (FormatException)
            {
                return format;
            }
        }

        /// <summary>
        /// Fixes for json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal static string FixForJson(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            return value.Replace("'", "&#39;").Replace("\"", "&#34;").Replace("'", "&#146;");
        }

        internal static bool IsRegex(this string value, string regularExpression)
        {
            var regex = new Regex(regularExpression);
            return regex.IsMatch(value);
        }

        #endregion

        #region Bool Extensions

        /// <summary>
        /// Toes the ability string.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        internal static string ToAbilityString(this bool value)
        {
            return (value) ? "Enabled" : "Disabled";
        }

        #endregion

        #region Enum Extensions

        /// <summary>
        /// Fixes the specified value, by replacing the underscore with a space.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal static string Fix(this Enum value)
        {
            return value.ToString().Replace("_", " ").SplitCamelCase();
        }

        #endregion

        #region Dictionary Extensions

        /// <summary>
        /// Gets the or add the object from a dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TDictionaryValue">The type of the dictionary value.</typeparam>
        /// <typeparam name="TActualValue">The type of the actual value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        public static TActualValue GetOrAdd<TKey, TDictionaryValue, TActualValue>(
            this IDictionary<TKey, TDictionaryValue> dictionary,
            TKey key,
            Func<TActualValue> newValue
            )
            where TActualValue : TDictionaryValue
        {
            TDictionaryValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                value = newValue.Invoke();
                dictionary.Add(key, value);
            }
            return (TActualValue)value;
        }

        public static TActualValue GetOrAdd<TKey, TActualValue>(
                this IDictionary dictionary,
            TKey key,
            Func<TActualValue> newValue
            )
        {
            if (dictionary[key] == null)
                dictionary.Add(key, newValue.Invoke());
            return (TActualValue)dictionary[key];
        }

        #endregion

        #region Object

        public static string AsSafeString(this object value)
        {
            if (value == null)
                return string.Empty;
            return value.ToString();
        }

        public static bool AsSafeBool(this object value)
        {
            if (value == null)
                return false;
            return (bool)value;
        }

        #endregion

        #region Guid Extensions

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Guid ToGuid(this string value)
        {
            try
            {
                return new Guid(value);
            }
            catch (Exception)
            {
                return Guid.NewGuid();
            }
        }

#pragma warning disable 168
        public static bool IsGuid(this string value)
        {
            try
            {
                var attempt = new Guid(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
#pragma warning restore 168

        #endregion

        #region Bytes

        public static byte[] ReadFull(this Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        #endregion
     
        #region Int Extensions

        /// <summary>
        /// Convert the object to int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static int ToInt(this object value)
        {
            return Convert.ToInt32(value);
        }

        #endregion

    }
}
