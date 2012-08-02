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
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace Logrythmik.Data
{
    public interface IRepository<TDataContext> : IDisposable
        where TDataContext : IDataContext, new() 
    {
        IDataContext DataContext { get; set; }

        /// <summary>
        /// Lists the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IQueryable List(Type type);

        /// <summary>
        /// Lists this type.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IQueryable<TEntity> List<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Gets the specified type, by id.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKeyType">The type of the key type.</typeparam>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        TEntity Get<TEntity, TKeyType>(TKeyType id)
            where TEntity : class;

        /// <summary>
        /// Finds the specified instance, using this selector.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        TEntity Find<TEntity>(Expression<Func<TEntity, bool>> selector)
             where TEntity : class;

        /// <summary>
        /// Adds the specified instance.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="instance">The instance.</param>
        void Add<TEntity>(TEntity instance)
             where TEntity : class;

        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="instance">The instance.</param>
        void Delete<TEntity>(TEntity instance)
             where TEntity : class;

        /// <summary>
        /// Executes the SQL command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        int ExecuteCommand(string command);

        /// <summary>
        /// Attaches the specified entity to the repository for tracking.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">The entity.</param>
        void Attach<TEntity>(TEntity entity);

        /// <summary>
        /// Submits the changes.
        /// </summary>
        void SubmitChanges();


        /// <summary>
        /// Gets the modified members.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        IEnumerable<ModifiedMemberInfo> GetModifiedMembers<TEntity>(TEntity entity);
    }
}