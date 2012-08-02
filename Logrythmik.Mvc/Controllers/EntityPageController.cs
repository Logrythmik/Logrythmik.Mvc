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
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Ipc.Mvc.Data;
using Ipc.Mvc.Web.Managers;
using Ipc.Mvc.Web.ViewModels;
using xVal.ServerSide;

namespace Ipc.Mvc.Web.Controllers
{
    public abstract class EntityPageController<T, TKeyType, TSiteType, TUserType> : BaseEntityController<T, TKeyType, TSiteType, TUserType>
        where T : class, new()
        where TSiteType : class, ISite
        where TUserType : class, IUser
    {
        // ReSharper disable Asp.NotResolved

        #region Constructor


        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPageController&lt;T, TKeyType, TSiteType, TUserType&gt;"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        protected EntityPageController(IEntityManager<T, TKeyType> manager) : base(manager)
        {
        }

        #endregion
        
        #region Actions

        // GET: /<T>/
        public virtual ActionResult Index()
        {
            return this.View(BuildListViewData(Manager.List()));
        }

        // GET: /<T>/Manage
        [Authorize("Admin")]
        public virtual ActionResult Manage()
        {
            return this.View(BuildListViewData(Manager.List()));
        }

        // GET: /<T>/<TEntity>/5
        [Authorize("Admin")]
        protected virtual ActionResult IndexBy<TEntity, TEntityKeyType>(TEntityKeyType id, Func<IEnumerable<T>> listDelegate)
            where TEntity: class, INamed
        {
            var entity = Manager.Repository.Get<TEntity, TEntityKeyType>(id);
            if (entity != null)
            {
                return this.View(BuildListViewData(listDelegate.Invoke())
                    .WithName(entity.Name));
            }
            return this.View(BuildListViewData(listDelegate.Invoke()));            
        }

        // GET: /<T>/Details/5
        public virtual ActionResult Details(TKeyType id)
        {
            var entity = Manager.GetReadOnly(id);
            if (entity == null)
                this.View(Constants.Error, GetViewData().WithMessage(
                    new ViewMessage("{0} not found with this ID.".ToFormat(this.InstanceName), MessageType.Error)));
            return this.View(BuildEditViewData(entity));
        }

        // GET: /<T>/New
        [Authorize("Admin")]
        public virtual ActionResult New()
        {
            return this.View(BuildNewViewData());
        }

        // POST: /<T>/New
        [Authorize("Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public virtual ActionResult New(T instance)
        {
            return New(instance, i => Manager.Add(i), null, null, null);
        }
        
        [NonAction]
        protected virtual ActionResult New(T instance, 
            Action<T> addAction,
            Action<T> postAddAction,
            Func<T, string> redirectAction,
            string redirectMessage)
        {
            if (!ModelState.IsValid)
                return this.View(BuildEditViewData(instance));

            try
            {
                if (addAction != null)
                   addAction.Invoke(instance);

                if (postAddAction != null)
                    postAddAction.Invoke(instance);
            }
            catch (RulesException exc)
            {
                exc.AddModelStateErrors(this.ModelState);
                return View(BuildEditViewData(instance));
            }
            catch(Exception exc)
            {
                return HandleException(BuildEditViewData(instance), exc);
            }

            this.AddRedirectMessage(new ViewMessage(redirectMessage ?? "{0} Created".ToFormat(this.InstanceName), MessageType.Success));

            if(redirectAction==null)
                return RedirectToAction(Constants.Views.Manage);

            return this.Redirect(redirectAction.Invoke(instance));
        }

        // GET: /<T>/Edit/5 
        [Authorize("Admin")]
        public virtual ActionResult Edit(TKeyType id)
        {
            var instance = Manager.Get(id);
            return this.View(BuildEditViewData(instance));
        }

        // POST: /<T>/Update/5
        [Authorize("Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public virtual ActionResult Edit(T updatedInstance, TKeyType id)
        {
            return Edit(updatedInstance, id, i => Manager.Update(i), null, null);
        }

        [NonAction]
        protected virtual ActionResult Edit(T updatedInstance, TKeyType id,
            Action<T> updateAction,
            Action<T> postEditAction,
            Func<T, string> redirectAction)
        {
            if (!ModelState.IsValid)
                return this.View(BuildEditViewData(updatedInstance));

            string redirectUrl;
            try
            {
                T instance = Manager.Get(id);
                this.UpdateModel(instance, this.UpdatableFields.ToArray());
                
                if(updateAction!=null)
                    updateAction.Invoke(instance);

                if(postEditAction!=null)
                    postEditAction.Invoke(instance);
                
                redirectUrl = (redirectAction!=null) ? redirectAction.Invoke(instance) : null;
            }
            catch (RulesException exc)
            {
                exc.AddModelStateErrors(this.ModelState);
                return View(BuildEditViewData(updatedInstance));
            }
            catch (Exception exc)
            {
                return HandleException(BuildEditViewData(updatedInstance), exc);
            }

            this.AddRedirectMessage(new ViewMessage("{0} Updated".ToFormat(this.InstanceName), MessageType.Success));

            if (redirectAction == null)
                return RedirectToAction(Constants.Views.Manage);

            return this.Redirect(redirectUrl);
        }
              



        #endregion

        #region Helper Methods

        /// <summary>
        /// Builds the edit view data.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        protected virtual BaseModelViewData<T, TSiteType, TUserType> BuildEditViewData(T instance)
        {
            var viewData = GetViewData<T>()
                .WithItem(instance);
            AppendLookupLists(viewData);
            return viewData;
        }

        /// <summary>
        /// Builds the edit view data.
        /// </summary>
        /// <param name="instanceList">The instance list.</param>
        /// <returns></returns>
        protected virtual BaseModelViewData<T, TSiteType, TUserType> BuildListViewData(IEnumerable<T> instanceList)
        {
            var viewData = GetViewData<T>()
                 .WithItems(instanceList);
            return viewData;
        }

        /// <summary>
        /// Builds the edit view data.
        /// </summary>
        /// <returns></returns>
        protected virtual BaseModelViewData<T, TSiteType, TUserType> BuildNewViewData()
        {
            var viewData = GetViewData<T>();
            AppendLookupLists(viewData);
            return viewData;
        }

        /// <summary>
        /// Appends any lookup lists T might need for editing
        /// </summary>
        /// <param name="viewData"></param>
        protected virtual void AppendLookupLists(BaseModelViewData<T, TSiteType, TUserType> viewData)
        {
            // find any properties that are attributed as a linq entity
            foreach (var property in typeof(T).GetProperties()
                .Where(property => property.PropertyType.IsLinqEntity()))
                viewData.WithData(property.PropertyType, Manager.Repository.List(property.PropertyType));
        }

        #endregion
    }
}
