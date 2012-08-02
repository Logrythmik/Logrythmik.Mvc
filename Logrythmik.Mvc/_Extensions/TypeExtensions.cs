#region Copyright
/*
 * Copyright (c) 2005-2009 IP Commerce, INC. - All Rights Reserved.
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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Web;
using Ipc.Common;
using Ipc.Data;
using Ipc.Mvc.Proxies;
using Ipc.Mvc.ViewModels.Attributes;

namespace Ipc.Mvc
{
    public static class TypeExtensions
    {
        private readonly static DataCacheProxy Cache = new DataCacheProxy();

        public static IEnumerable<PropertyInfo> ListViewableProperties(this Type type, HttpContextBase context)
        {
            return type.GetProperties().Where(propertyInfo =>
                                              (!propertyInfo.HasAttribute<ScaffoldColumnAttribute>() ||
                                               propertyInfo.GetAttribute<ScaffoldColumnAttribute>().Scaffold) &&
                                              (!propertyInfo.HasAttribute<AuthorizeFieldAttribute>() ||
                                               propertyInfo.GetAttribute<AuthorizeFieldAttribute>().ShowField(context))
                );           
        }

        public static string[] UpdatableFields<T>(this T instance)
            where T: class
        {
            return Cache.GetCache(
                 "{0}:UpdatableFields".ToFormat(typeof(T)),
                 () =>
                     {
                         var list = new List<string>();
                         var metaType = instance.GetType().GetMetaClass();
                         foreach (var propertyInfo in metaType.GetProperties())
                         {
                             if(propertyInfo.HasAttribute<ScaffoldColumnAttribute>() && 
                                 propertyInfo.GetAttribute<ScaffoldColumnAttribute>().Scaffold == false)
                                 continue;

                             list.Add(propertyInfo.HasAttribute<LookupListAttribute>()
                                          ? propertyInfo.GetAttribute<LookupListAttribute>().AssignedPropertyName
                                          : propertyInfo.Name);
                         }
                         return list.ToArray();
                     }
                 );
        }

        public static string GetRootUrl(this Uri uri)
        {
            return string.Format("{0}://{1}", uri.Scheme, uri.Host);
        }


    }
}