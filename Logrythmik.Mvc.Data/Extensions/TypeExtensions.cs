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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using AssociationAttribute = System.Data.Linq.Mapping.AssociationAttribute;

namespace Logrythmik.Data
{
    public static class TypeExtensions
    {
        #region Type Extensions

   

        public static bool IsLinqEntity(this Type type)
        {
            return type.GetAttribute<TableAttribute>() != null;
        }

        public static bool IsEnumerable(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static string DisplayName(this Type type)
        {
           var attr = type.GetAttribute<DisplayNameAttribute>();
            if (attr != null)
                return attr.DisplayName;
            return type.Name.SplitCamelCase();
        }

        public static Type GetMetaClass(this Type type)
        {
            var metaAtt = type.GetAttribute<MetadataTypeAttribute>();
            if (metaAtt != null)
                return metaAtt.MetadataClassType;
            return type;
        }

        public static TAttribute GetAttribute<TAttribute>(this Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(TAttribute), true);
            if (attributes.Length == 0) return default(TAttribute);
            return (TAttribute)attributes[0];
        }

        public static PropertyInfo GetPrimaryKey<T>(this Type entityType)
        {
            foreach (PropertyInfo property in entityType.GetProperties())
            {
                var attributes = (ColumnAttribute[])property.GetCustomAttributes(typeof(ColumnAttribute), true);
                if (attributes.Length == 1)
                {
                    ColumnAttribute columnAttribute = attributes[0];
                    if (columnAttribute.IsPrimaryKey)
                    {
                        if (property.PropertyType != typeof(T))
                        {
                            throw new ApplicationException(string.Format(
                                "Primary key, '{0}', of type '{1}' is not {2}", property.Name, entityType, typeof(T).Name));
                        }
                        return property;
                    }
                }
            }
            throw new ApplicationException(string.Format(
                "No primary key defined for type {0}", entityType.Name));
        }

        public static string PrimaryKeyIdField(this Type type)
        {
            if (type.IsLinqEntity())
            {
                var propertyInfo = type.GetProperties().GetPropertyWithAttributeValue<ColumnAttribute>(c => c.IsPrimaryKey);
                if (propertyInfo != null)
                    return propertyInfo.Name;
            }

            return type.Name + "Id";
        }

        public static IEnumerable<PropertyInfo> PropertiesWithAttribute(this Type type, Type attributeType)
        {
            return type.GetProperties().Where(property => property.HasAttribute(attributeType));
        }

        public static IEnumerable<string> ListPropertyNames(this Type type)
        {
            return type.GetProperties().Select(p => p.Name);
        }

        #endregion

        #region PropertyInfo Extensions

        public static bool IsPrimaryKey(this PropertyInfo propertyInfo)
        {
            var columnAttribute = propertyInfo.GetAttribute<ColumnAttribute>();
            if (columnAttribute == null) return false;
            return columnAttribute.IsPrimaryKey;
        }

        public static bool IsForeignKey(this PropertyInfo propertyInfo)
        {
            var association = propertyInfo.GetAttribute<AssociationAttribute>();
            if (association == null) return false;
            return association.IsForeignKey;
        }

        public static bool IsAssociation(this PropertyInfo propertyInfo)
        {
            var association = propertyInfo.GetAttribute<AssociationAttribute>();
            return association != null;
        }
       
        public static string ForeignKeyIdField(this PropertyInfo propertyInfo)
        {
            var association = propertyInfo.GetAttribute<AssociationAttribute>();
            if (association == null) return null;
            return association.ThisKey;
        }

        public static PropertyInfo GetMetaProperty(this PropertyInfo propertyInfo)
        {
            var type = propertyInfo.DeclaringType.GetMetaClass();
            return type.GetProperty(propertyInfo.Name) ?? propertyInfo;
        }

        /// <summary>
        /// Gets the display name for a property
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        public static string DisplayName(this PropertyInfo propertyInfo)
        {
            var type = propertyInfo.DeclaringType.GetMetaClass();
            var metaProp = type.GetProperty(propertyInfo.Name);
            if (metaProp != null)
            {
                var metaAttr = metaProp.GetAttribute<DisplayNameAttribute>();
                if (metaAttr != null)
                    return metaAttr.DisplayName;
            }
            var attr = propertyInfo.GetAttribute<DisplayNameAttribute>();
            if (attr != null)
                return attr.DisplayName;
            return propertyInfo.Name.SplitCamelCase();
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(this PropertyInfo propertyInfo)
        {
            var attributes = propertyInfo.GetCustomAttributes(typeof(TAttribute), true);
            if (attributes.Length == 0) return default(TAttribute);
            return (TAttribute)attributes[0];
        }


        /// <summary>
        /// Determines whether the specified property info has attribute.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>
        /// 	<c>true</c> if the specified property info has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute(this PropertyInfo propertyInfo, Type attributeType)
        {
            return propertyInfo.GetCustomAttributes(attributeType, true).Any();
        }

        /// <summary>
        /// Determines whether the specified property info has attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns>
        /// 	<c>true</c> if the specified property info has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute<TAttribute>(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes(typeof(TAttribute), true).Any();
        }

        #endregion

        #region Misc
              


        public static string GetPropertyName<T, TPropertyType>(this Expression<Func<T, TPropertyType>> property)
        {
            return property.GetProperty().Name;
        }

        public static TPropertyType GetPropertyValue<T, TPropertyType>(this PropertyInfo property, T item)
            where T : class
        {
            object value = default(TPropertyType);
            if (item != null)
                value = property.GetValue(item, null);
            return (TPropertyType)value;
        }

        public static PropertyInfo GetProperty<T, TPropertyType>(this Expression<Func<T, TPropertyType>> propertyLambda)
        {
            var type = typeof(T);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda));

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda,
                    type));

            return propInfo;
        }

        public static PropertyInfo GetPropertyWithAttributeValue<TAttribute>(
            this IEnumerable<PropertyInfo> properties, 
            Func<TAttribute, bool> findPredicate)
            where TAttribute: Attribute
        {
            var property = from p in properties
                           where p.HasAttribute<TAttribute>() &&
                           findPredicate.Invoke(p.GetAttribute<TAttribute>())
                           select p;
            
            return property.FirstOrDefault();
        }

        #endregion

        public static object GetNull(this Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

    }
}