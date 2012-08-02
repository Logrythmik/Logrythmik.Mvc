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
using System.Collections.Generic;
using System.Linq;

namespace Logrythmik.Data
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Use instead of a foreach loop e.g.
        /// MyCollection.Each(item => DoSomething(item));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
                action(item);
        }

        /// <summary>
        /// Convenient replacement for a range 'for' loop. e.g. return an array of int from 10 to 20:
        /// int[] tenToTwenty = 10.to(20).ToArray();
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<int> To(this int from, int to)
        {
            for (int i = from; i <= to; i++)
                yield return i;
        }

        public static void ToConsole<T>(this IEnumerable<T> list)
        {
            list.ForEach(n => Console.Write("{0} ", (n)));
        }

        public static List<T> ToList<T>(this IEnumerable source)
        {
            return source.Cast<T>().ToList();
        }

        public static List<TTo> ToList<TFrom, TTo>(this IEnumerable source, Func<TFrom, TTo> convert)
        {
            return (from TFrom t in source select convert(t)).ToList();
        }

        public static IEnumerable<T> AtOddPositions<T>(this IEnumerable<T> list)
        {
            bool odd = false; // 0th position is even
            foreach (T item in list)
            {
                odd = !odd;
                if (odd)
                    yield return item;
            }
        }

        public static IEnumerable<T> AtEvenPositions<T>(this IEnumerable<T> list)
        {
            bool even = true; // 0th position is even
            foreach (T item in list)
            {
                even = !even;
                if (even)
                    yield return item;
            }
        }

        public static bool DoesAnyKeyHavePrefix<TValue>(this IDictionary<string, TValue> dictionary, string prefix)
        {
            return FindKeysWithPrefix(dictionary, prefix).Any();
        }

        public static IEnumerable<KeyValuePair<string, TValue>> FindKeysWithPrefix<TValue>(this IDictionary<string, TValue> dictionary, string prefix)
        {
            TValue exactMatchValue;
            if (dictionary.TryGetValue(prefix, out exactMatchValue))
            {
                yield return new KeyValuePair<string, TValue>(prefix, exactMatchValue);
            }

            foreach (var entry in dictionary)
            {
                string key = entry.Key;

                if (key.Length > prefix.Length && key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    switch (key[prefix.Length])
                    {
                        case '[':
                        case '.':
                            yield return entry;
                            break;
                    }
                }
            }
        }
    }


}

