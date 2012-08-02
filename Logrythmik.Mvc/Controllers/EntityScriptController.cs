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
using Ipc.Mvc.Data;
using Ipc.Mvc.Web.Managers;
using xVal.ServerSide;

namespace Ipc.Mvc.Web.Controllers
{
    public abstract class EntityScriptController<T, TKeyType, TSiteType, TUserType> : BaseEntityController<T, TKeyType, TSiteType, TUserType>
        where T : class, new()
        where TSiteType : class, ISite
        where TUserType : class, IUser
    {
        // ReSharper disable Asp.NotResolved

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityScriptController&lt;T, TKeyType, TSiteType, TUserType&gt;"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        protected EntityScriptController(IEntityManager<T, TKeyType> manager)
            : base(manager)
        {
        }

        #endregion


        #region Script Actions

        // POST: /<T>/List
        [Authorize("Admin")]
        public virtual PartialViewResult List()
        {
            return PartialView(Manager.List());
        }

   
        // POST: /<T>/Get/5
        [Authorize("Admin")]
        protected virtual JsonResult Get<TViewModel>(TKeyType id, Func<T, TViewModel> mappedResult)
        {
            try
            {
                var instance = Manager.Get(id);
                if (instance == null)
                    return Json(new
                    {
                        success = false, id,
                        message = "{0} not found with this ID. ".ToFormat(this.InstanceName)
                    });
                

                return Json(new { success = true, id, item = mappedResult(instance) });
            }
            catch (Exception exc)
            {
                exc.Log();
                return Json(new
                {
                    success = false, id,
                    message = ((this.CurrentUser.IsAdmin) ? exc.Message : "An error occured.")
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: /<T>/Add
        [Authorize("Admin")]
        public virtual JsonResult Add(T instance)
        {
            return Add(instance, i => Manager.Add(i), null);
        }
        
        [NonAction]
        protected virtual JsonResult Add(T instance, 
            Action<T> addAction,
            Action<T> postAddAction)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, item = instance, messsage = "There were validation errors." });

            try
            {
                if (addAction != null)
                   addAction.Invoke(instance);

                if (postAddAction != null)
                    postAddAction.Invoke(instance);

                return Json(new { success = true, item = instance });
            }
            catch (RulesException exc)
            {
                exc.AddModelStateErrors(this.ModelState);
                return Json(new { success = false, item = instance, messsage = "There were validation errors." });
            }
            catch(Exception exc)
            {
                exc.Log();
                return Json(new { 
                    success = false, 
                    item = instance, 
                    message = ((this.CurrentUser.IsAdmin) ? exc.Message : "An error occured.") });
            }
        }

        // POST: /<T>/Update/5
        [Authorize("Admin")]
        protected virtual JsonResult Update<TViewModel>(T updatedInstance, TKeyType id, Func<T, TViewModel> mappedResult)
        {
            return Update(updatedInstance, id, i => Manager.Update(i), null, mappedResult);
        }

        [NonAction]
        protected virtual JsonResult Update<TViewModel>(T updatedInstance, TKeyType id,
            Action<T> updateAction,
            Action<T> postEditAction, 
            Func<T, TViewModel> mappedResult)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, item = updatedInstance, messsage = "There were validation errors." });

            try
            {
                T instance = Manager.Get(id);
                this.UpdateModel(instance, this.UpdatableFields.ToArray());
                
                if(updateAction!=null)
                    updateAction.Invoke(instance);

                if(postEditAction!=null)
                    postEditAction.Invoke(instance);

                return Json(new { success = true, item = mappedResult(instance) });
            }
            catch (RulesException exc)
            {
                exc.AddModelStateErrors(this.ModelState);
                return Json(new { success = false, item = updatedInstance, messsage = "There were validation errors." });
            }
            catch (Exception exc)
            {
                exc.Log();
                return Json(new { 
                    success = false, 
                    item = updatedInstance,
                    messsage = ((this.CurrentUser.IsAdmin) ? exc.Message : "An error occured.")
                });
            }
        }


        #endregion

        
    }
}
