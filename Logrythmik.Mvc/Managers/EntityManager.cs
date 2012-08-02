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
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using Logrythmik.Data;

namespace Logrythmik.Mvc.Managers
{
    public class EntityManager<TEntityType, TKeyType, TDataContext> : BaseManager<TDataContext>, IEntityManager<TEntityType, TKeyType, TDataContext>
        where TEntityType : class
        where TDataContext : IDataContext, new() 
    {
        #region Properties

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <value>The repository.</value>
        public IEntityRepository<TEntityType, TDataContext> Repository
        {
            get { return _Repository; }
        }
        private readonly IEntityRepository<TEntityType, TDataContext> _Repository;

        /// <summary>
        /// Gets the load options for read-only/cached calls.
        /// Override this in the concrete class to specify 
        /// items needed.
        /// </summary>
        /// <value>The load options.</value>
        public virtual DataLoadOptions LoadOptions { get; set; }
               

        /// <summary>
        /// Gets the read only context.
        /// </summary>
        /// <value>The read only context.</value>
        public IReadOnlyDataContext ReadOnlyContext
        {
            get { 
                return _ReadOnlyContext ?? 
                    (_ReadOnlyContext = this.ReadOnlyDataContextProvider.GetNewContext(this.LoadOptions)); 
            }
        }
        private IReadOnlyDataContext _ReadOnlyContext;

        public IReadOnlyDataContext GetReadOnlyContext(DataLoadOptions loadOptions)
        {
            return this.ReadOnlyDataContextProvider.GetNewContext(loadOptions);
        }
                
        #endregion

        #region Constructor


        /// <summary>
        /// Initializes a new instance of the <see cref="EntityManager&lt;TEntityType, TKeyType, TDataContext&gt;"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public EntityManager(IEntityRepository<TEntityType, TDataContext> repository)
        {
            _Repository = repository;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Gets the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public virtual TEntityType Get(TKeyType id)
        {
            return _Repository.Get(id);
        }

        /// <summary>
        /// Gets the read only.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public virtual TEntityType GetReadOnly(TKeyType id)
        {
            return this.DataCache.GetCache(
                ItemKey(id), 
                () => this.ReadOnlyContext.GetEntity<TEntityType, TKeyType>(id), 
                DateTime.Now.AddMinutes(15));
        }

        /// <summary>
        /// Gets the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public virtual TEntityType Find(Expression<Func<TEntityType,bool>> selector)
        {
            return _Repository.Find<TEntityType>(selector);
        }


        /// <summary>
        /// Lists this instance.
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntityType> List()
        {
            if (typeof(ISortable).IsAssignableFrom(typeof(TEntityType)))
                return _Repository.List().OrderBy(i => ((ISortable)i).SortOrder);
            return _Repository.List<TEntityType>();
        }

        /// <summary>
        /// Lists the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public virtual IEnumerable List(Type type)
        {
            return _Repository.List(type);
        }

        /// <summary>
        /// Adds the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public virtual void Add(TEntityType instance)
        {
            _Repository.Add(instance);
            _Repository.SubmitChanges();
        }

        /// <summary>
        /// Updates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public virtual void Update(TEntityType instance)
        {
            _Repository.SubmitChanges();
            this.DataCache.RemoveCache(this.ItemKey(instance));
        }

        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public virtual void Delete(TEntityType instance)
        {
            _Repository.Delete(instance);
            _Repository.SubmitChanges();
        }

        /// <summary>
        /// Submits the changes.
        /// </summary>
        public void SubmitChanges()
        {
            _Repository.SubmitChanges();
        }

        /// <summary>
        /// Clears the item cache.
        /// </summary>
        /// <param name="id">The id.</param>
        public void ClearItemCache(TKeyType id)
        {
            this.DataCache.RemoveCache(this.ItemKey(id));
        }

        #endregion

        #region Protected Helpers

        protected string ItemKey(TKeyType id)
        {
            return string.Format("{0}:{1}", _KeyBase, id);
        }

        protected string ItemKey(TEntityType instance)
        {
            var propertyInfo = typeof(TEntityType).GetPrimaryKey<TKeyType>();
            var id = propertyInfo.GetValue(instance, null);
            return string.Format("{0}:{1}", _KeyBase, id);
        }

        private readonly string _KeyBase = typeof(TEntityType).Name;

        #endregion

        public void Dispose()
        {
            try
            {
                if(_ReadOnlyContext !=null)
                    _ReadOnlyContext.Dispose();
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            {
            }
            // ReSharper restore EmptyGeneralCatchClause
        }
        
    }
}
