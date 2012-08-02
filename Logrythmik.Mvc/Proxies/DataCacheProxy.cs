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
using System.Web;
using System.Web.Caching;

namespace Logrythmik.Mvc.Proxies
{
    public class DataCacheProxy : IDataCacheProxy
    {
        #region SetCache Methods

        /// <summary>
        /// Places a provided object in the cache, indexed with a specified key.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="obj">The object.</param>
        public void SetCache(string cacheKey, object obj)
        {
            if (cacheKey == null || obj == null)
                return;

            var cache = HttpRuntime.Cache;
            cache.Insert(cacheKey, obj);
        }

        /// <summary>
        /// Places a provided object in the cache along with dependencies,
        /// indexed with a specified key.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="obj">The in object.</param>
        /// <param name="dependency">The dependency.</param>
        public void SetCache(string cacheKey, object obj, CacheDependency dependency)
        {
            if (cacheKey == null || obj == null)
                return;

            var cache = HttpRuntime.Cache;
            cache.Insert(cacheKey, obj, dependency);
        }

        /// <summary>
        /// Places a provided object in the cache, indexed with a specified key.
        /// The object will be removed from the cache if not accessed within a provided TimeSpan.
        /// </summary>
        /// <param name="cacheKey">Key to access cached object.</param>
        /// <param name="obj">Object to cache.</param>
        /// <param name="slidingExpiration">The sliding expiration.</param>
        public void SetCache(string cacheKey, object obj, int slidingExpiration)
        {
            if (cacheKey == null || obj == null)
                return;

            var cache = HttpRuntime.Cache;
            cache.Insert(cacheKey, obj, null, DateTime.MaxValue, TimeSpan.FromSeconds(slidingExpiration));
        }

        /// <summary>
        /// Places a provided object in the cache, indexed with a specified key.
        /// The object will be removed from the cache by a certain DateTime.
        /// </summary>
        /// <param name="cacheKey">Key to access cached object.</param>
        /// <param name="obj">Object to cache.</param>
        /// <param name="absoluteExpiration">The absolute expiration.</param>
        public void SetCache(string cacheKey, object obj, DateTime absoluteExpiration)
        {
            if (cacheKey == null || obj == null)
                return;

            var cache = HttpRuntime.Cache;
            cache.Insert(cacheKey, obj, null, absoluteExpiration, TimeSpan.Zero);
        }

        /// <summary>
        /// Sets the cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="value">The value.</param>
        /// <param name="absoluteExpiration">The absolute expiration.</param>
        /// <param name="itemRemovedCallback">The item removed callback.</param>
        public void SetCache(string cacheKey, object value, DateTime absoluteExpiration,
            CacheItemRemovedCallback itemRemovedCallback)
        {
            if (cacheKey == null || value == null)
                return;

            var cache = HttpRuntime.Cache;
            cache.Insert(cacheKey, value, null, absoluteExpiration, 
                             TimeSpan.Zero, CacheItemPriority.Normal,
                             itemRemovedCallback);
        }



        #endregion

        #region GetCache Methods

        /// <summary>
        /// Gets a value from the cache given key.
        /// </summary>
        /// <param name="cacheKey">The key for the value to retrieve.</param>
        /// <returns>An object from the cache.</returns>
        public object GetCache(string cacheKey)
        {
            var cache = HttpRuntime.Cache;
            return cache[cacheKey];
        }

        /// <summary>
        /// Gets the cached item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns></returns>
        public T GetCache<T>(string cacheKey)
        {
            var cache = HttpRuntime.Cache;
            return (T)cache[cacheKey];
        }

        /// <summary>
        /// Gets the cached item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="functionToSet">The function to set.</param>
        /// <returns></returns>
        public T GetCache<T>(string key, Func<T> functionToSet)
        {
            if (GetCache(key) == null)
                SetCache(key, functionToSet.Invoke());

            return GetCache<T>(key);
        }

        /// <summary>
        /// Gets the cached item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="functionToSet">The function to set.</param>
        /// <param name="absoluteExpiration">The absolute expiration.</param>
        /// <returns></returns>
        public T GetCache<T>(string key, Func<T> functionToSet, DateTime absoluteExpiration)
        {
            if (GetCache(key) == null)
                SetCache(key, functionToSet.Invoke(), absoluteExpiration);

            return GetCache<T>(key);
        }

        /// <summary>
        /// Gets the cached item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="functionToSet">The function to set.</param>
        /// <param name="slidingExpiration">The sliding expiration.</param>
        /// <returns></returns>
        public T GetCache<T>(string key, Func<T> functionToSet, int slidingExpiration)
        {
            if (GetCache(key) == null)
                SetCache(key, functionToSet.Invoke(), slidingExpiration);

            return GetCache<T>(key);
        }

        /// <summary>
        /// Gets the cached item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="functionToSet">The function to set.</param>
        /// <param name="cacheDependency">The cache dependency.</param>
        /// <returns></returns>
        public T GetCache<T>(string key, Func<T> functionToSet, CacheDependency cacheDependency)
        {
            if (GetCache(key) == null)
                SetCache(key, functionToSet.Invoke(), cacheDependency);

            return GetCache<T>(key);
        }

        #endregion

        #region Remove / Clear Methods

        /// <summary>
        /// Removes an item from the cache given key.
        /// </summary>
        /// <param name="cacheKey">Key to access cached object.</param>
        public void RemoveCache(string cacheKey)
        {
            var cache = HttpRuntime.Cache;
            if (cache[cacheKey] != null)
                cache.Remove(cacheKey);
        }

        /// <summary>
        /// Clears the cache by key.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        public void ClearCacheByKey(string cacheKey)
        {
            var cache = HttpRuntime.Cache;
            foreach (DictionaryEntry entry in cache)
                if (entry.Key.ToString().StartsWith(cacheKey))
                    cache.Remove((string)entry.Key);
        }

        /// <summary>
        /// Clears the global cache.
        /// </summary>
        public void ClearGlobalCache()
        {
            // clear entire cache
            var cache = HttpRuntime.Cache;
            foreach (DictionaryEntry entry in cache)
                cache.Remove((string)entry.Key);
        }



        #endregion

    }
}
