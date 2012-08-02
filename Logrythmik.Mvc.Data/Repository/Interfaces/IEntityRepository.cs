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
using System.Linq;
using System.Linq.Expressions;

namespace Logrythmik.Data
{
    public interface IEntityRepository<TInstance, TDataContext> : IRepository<TDataContext>
        where TInstance: class
        where TDataContext : IDataContext, new() 
    {
        #region Instance

        /// <summary>
        /// Lists this instance.
        /// </summary>
        /// <returns></returns>
        IQueryable<TInstance> List();

        /// <summary>
        /// Gets the specified instance by id.
        /// </summary>
        /// <typeparam name="TKeyType">The type of the key type.</typeparam>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        TInstance Get<TKeyType>(TKeyType id);

        /// <summary>
        /// Finds the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        TInstance Find(Expression<Func<TInstance, bool>> selector);

        /// <summary>
        /// Adds the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        void Add(TInstance instance);

        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        void Delete(TInstance instance);

        #endregion
    }

    public interface IEntityRepository
    {
        #region Instance

        /// <summary>
        /// Lists this instance.
        /// </summary>
        /// <returns></returns>
        IQueryable List();

        /// <summary>
        /// Gets the specified instance by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        object Get(int id);

     
        /// <summary>
        /// Adds the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        void Add(object instance);

        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        void Delete(object instance);


        #endregion
    }
}