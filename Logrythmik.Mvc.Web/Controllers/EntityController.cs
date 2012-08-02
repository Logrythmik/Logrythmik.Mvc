#region Copyright
/*
 * Copyright (c) 2004-2011 Logrythmik Consulting, LLC. - All Rights Reserved.
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
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

using Logrythmik.Data;

using Logrythmik.Mvc.Binders;
using Logrythmik.Mvc.Controllers.Exceptions;
using Logrythmik.Mvc.Managers;
using Logrythmik.Mvc.ViewModels;

namespace Logrythmik.Mvc.Controllers
{
    public abstract class EntityController<TViewType, TEntityType, TKeyType, TDataContext> : BaseController
        where TViewType : ViewModel<TViewType, TEntityType, TKeyType, TDataContext>, new()
        where TDataContext : IDataContext, new()
        where TEntityType : class, new()
    {
        #region Constructor

        private readonly object _Lock = new object();

        protected EntityController() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityController&lt;TViewType, TEntityType, TKeyType, TDataContext&gt;"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        protected EntityController(IEntityManager<TEntityType, TKeyType, TDataContext> manager)
        {
            Manager = manager;
            Repository = manager.Repository;

            try
            {
                lock (_Lock)
                {
                    // Setup the ViewModel Binder
                    if (!this.Binders.ContainsKey(typeof (TViewType)))
                        this.Binders.Add(typeof (TViewType),
                                         new ViewModelBinder<TViewType, TEntityType, TKeyType, TDataContext>());
                }
            }
            catch (Exception exc)
            {
                exc.Log();
            }
        }

        #endregion

        #region Properties

        public IRepository<TDataContext> Repository;

        #region Protected Properties

        public IEntityManager<TEntityType, TKeyType, TDataContext> Manager { get; set; }

        /// <summary>
        /// Gets the name of the instance.
        /// </summary>
        /// <value>The name of the instance.</value>
        protected string InstanceName
        {
            get { return typeof(TEntityType).DisplayName(); }
        }

        #endregion

        #endregion

        #region Actions Helpers

        #region Get

        /// <summary>
        /// This Action-helper is shared by both Edit and Details
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="viewCondition">The view condition.</param>
        /// <param name="dressEntity">The action to take on the entity before returning it to the view.</param>
        /// <param name="dressView">The dress view.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        protected ActionResult EntityGet(
            TKeyType id,
            Func<TEntityType, bool> viewCondition = null,
            Action<TEntityType> dressEntity = null,
            Action<TViewType> dressView = null,
            string viewName = null)
        {
            TEntityType entity = null;

            try
            {
                entity = this.Manager.Get(id);
                var condition = viewCondition == null || viewCondition(entity);
                if (entity == null || !condition)
                    return PageNotFound();

                if (dressEntity != null)
                    dressEntity(entity);
            }
            catch (PartialSuccessException exc)
            {
                // message and fall through
                this.AddWarningMessage(exc.Message);
            }
            catch (Exception exc)
            {
                HandleEntityException(new TViewType(), exc);
            }


            return ViewModel(viewName, entity);
        }

        #endregion

        #region Lists

        protected ActionResult EntityList(
            Action<TEntityType> dressEntity = null,
            Func<IEnumerable<TEntityType>> entityList = null,
            string viewName = null)
        {
            IEnumerable<TEntityType> list = null;

            try
            {
                list = (entityList != null) ? entityList() : this.Manager.List();
                if (dressEntity != null && list != null)
                    list.ForEach(dressEntity);
            }
            catch (PartialSuccessException exc)
            {
                // message and fall through   
                this.AddWarningMessage(exc.Message);
            }
            catch (Exception exc)
            {
                return HandleException(new List<TViewType>(), exc);
            }

            return ViewModel(viewName, list);
        }

        protected ActionResult EntityListBy<TByEntity, TEntityByKeyType>(
            TEntityByKeyType id,
            Func<TByEntity, IEnumerable<TEntityType>> listDelegate,
            Action<TEntityType> dressEntity = null,
            string viewName = null)
            where TByEntity : class, INamed
        {
            var list = new List<TEntityType>();
            try
            {
                var entity = this.Manager.Repository.Get<TByEntity, TEntityByKeyType>(id);
                if (entity == null)
                    return PageNotFound();

                list = listDelegate(entity).ToList();

                if (dressEntity != null)
                    list.ForEach(dressEntity);

                return ViewModel(viewName, list)
                    .WithPageTitle("{0}: {1}s".ToFormat(entity.Name, this.InstanceName));
               
            }
            catch (PartialSuccessException exc)
            {
                // message and fall through
                this.AddWarningMessage(exc.Message);
            }
            catch (Exception exc)
            {
                var result = HandleException(list, exc);
                result.ViewName = viewName;
                return result;
            }

            return ViewModel(viewName, list);
        }

        #endregion

        #region Create

        /// <summary>
        /// Returns a new view-model to the view.
        /// </summary>
        /// <returns></returns>
        protected ActionResult EntityCreate(
            string viewName = null,
            TViewType newEntity = null)
        {
            return ViewModel(viewName, newEntity ?? new TViewType());
        }


        /// <summary>
        /// This is the action handler for Create
        /// </summary>
        /// <param name="viewInstance">The view instance.</param>
        /// <param name="redirectAction">The action to redirect to on success.</param>
        /// <param name="additionalMapAction">The map action, leave null to if Mapping is enough.</param>
        /// <param name="addAction">The add action, leave null to just add.</param>
        /// <param name="postAddAction">The post add action.</param>
        /// <param name="redirectMessage">The redirect message.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        protected ActionResult EntityCreate(TViewType viewInstance,
            Func<TEntityType, ActionResult> redirectAction,
            Action<TViewType, TEntityType> additionalMapAction = null,
            Action<TEntityType> addAction = null,
            Action<TEntityType> postAddAction = null,
            string redirectMessage = null,
            string viewName = null)
        {
            if (!ModelState.IsValid)
                return ViewModel(viewName, viewInstance, true);

            var entity = new TEntityType();

            try
            {
                entity = viewInstance.CreateModel();

                if (additionalMapAction != null)
                    additionalMapAction(viewInstance, entity);

                if (addAction != null)
                    addAction(entity);
                else
                    Manager.Add(entity);

                if (postAddAction != null)
                    postAddAction(entity);

                Manager.Update(entity);
            }
            catch (PartialSuccessException exc)
            {
                // message and fall through   
                this.AddRedirectMessage(new ViewMessage(exc.Message, MessageType.Warning));
            }
            catch (Exception exc)
            {
                return HandleEntityException(viewInstance, exc, viewName);
            }

            this.AddRedirectMessage(new ViewMessage(redirectMessage ?? "{0} Created".ToFormat(this.InstanceName), MessageType.Success));

            return this.RedirectToAction(redirectAction(entity));
        }

        #endregion

        #region Edit

        /// <summary>
        /// Handles the edit action for entities and view.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <param name="updatedView">The updated view.</param>
        /// <param name="redirectAction">The redirect action.</param>
        /// <param name="viewCondition">The view condition.</param>
        /// <param name="additionalMapAction">The update action, leave null if mapping is enough.</param>
        /// <param name="postEditAction">The post edit action.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        protected ActionResult EntityEdit(TKeyType id, TViewType updatedView,
            Func<TEntityType, ActionResult> redirectAction,
            Func<TEntityType, bool> viewCondition = null,
            Action<TViewType, TEntityType> additionalMapAction = null,
            Action<TEntityType> postEditAction = null,
            string viewName = null)
        {
            if (!ModelState.IsValid)
                return ViewModel(viewName, updatedView, true);

            var instance = this.Manager.Get(id);
            if (instance == null)
                return PageNotFound();

            try
            {
                updatedView.UpdateModel(instance);

                if (additionalMapAction != null)
                    additionalMapAction(updatedView, instance);

                this.Manager.Update(instance);

                if (postEditAction != null)
                    postEditAction(instance);
            }
            catch (PartialSuccessException exc)
            {
                // message and fall through
                this.AddRedirectMessage(new ViewMessage(exc.Message, MessageType.Warning));
            }
            catch (Exception exc)
            {
                return HandleEntityException(updatedView, exc, viewName);
            }

            this.AddRedirectMessage(new ViewMessage("{0} Updated".ToFormat(this.InstanceName), MessageType.Success));
            return this.RedirectToAction(redirectAction(instance));
        }



        #endregion

        #region Delete

        // TODO: Refactor to allow for Get-Post paradigm for progressive enhancement
        protected JsonResult EntityDelete(TKeyType id)
        {
            return this.EntityScriptAction(id, entity =>
                Manager.Delete(entity),
                "{0} Deleted".ToFormat(this.InstanceName)
                );
        }

        #endregion

        #region Helper Actions

        /// <summary>
        /// An edit action, for Ajax
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="action">The action.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected JsonResult EntityScriptAction(
            TKeyType instanceId,
            Action<TEntityType> action,
            string message)
        {
            try
            {
                var instance = Manager.Get(instanceId);
                if (instance == null)
                    return Json(new
                    {
                        success = false,
                        id = instanceId,
                        message = "{0} not found with this ID. ".ToFormat(this.InstanceName)
                    });

                action(instance);

                Manager.Update(instance);

                return Json(new { success = true, id = instanceId, message });

            }
            catch (PartialSuccessException exc)
            {
                return Json(new
                {
                    success = true,
                    id = instanceId,
                    message = exc.Message
                });
            }
            catch (MessageException exc)
            {
                return Json(new
                {
                    success = false,
                    id = instanceId,
                    message = exc.Message
                });
            }
            catch (Exception exc)
            {
                exc.Log();
                return Json(new
                {
                    success = false,
                    id = instanceId,
                    message = "An error occurred. " + ((this.Request.IsLocal) ? exc.Message : string.Empty)
                });
            }
        }


        /// <summary>
        /// Will return the logo binary result for an object if it IHasLogo
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="noImageRelativePath">The no image relative path.</param>
        /// <param name="noImageContentType">Type of the content.</param>
        /// <param name="cache">if set to <c>true</c> [cache].</param>
        /// <returns></returns>
        protected FileResult EntityImage(TKeyType id,
            string noImageRelativePath = "~/Content/Images/Spacer.gif",
            string noImageContentType = "image/gif",
            bool cache = true)
        {
            var entity = Manager.GetReadOnly(id);
            if (entity == null)
                return null;

            var logoModel = entity as IHasImage;
            if (logoModel == null || logoModel.Image == null)
                return File(Server.MapPath(noImageRelativePath), noImageContentType);

            if (cache)
            {
                Response.Cache.SetCacheability(HttpCacheability.Public);
                Response.Cache.SetExpires(DateTime.Now.AddMonths(1));
            }

            return File(logoModel.Image.ToArray(), logoModel.ImageExtension);
        }

        protected PartialViewResult EntityPartialAction<TResultModel>(
            TKeyType id,
            string partialView,
            Action<TEntityType> action,
            Func<TEntityType, TResultModel> resultModel
            )
        {
            var instance = Manager.Get(id);
            if (instance == null)
                return PageNotFound() as PartialViewResult;

            try
            {
                action(instance);

                Manager.Update(instance);

                // Reset the container property, to ensure access to external data
                ViewBag.Container = new TViewType()
                    .MapFromModel(instance)
                    .Setup(this);
            }
            catch (MessageException exc)
            {
                ViewBag.Error = exc.Message;
            }
            catch (Exception exc)
            {
                ViewBag.Error = this.Request.IsLocal ? exc.Message : "An Error Occurred";
            }

            return PartialView(
                partialView,
                resultModel(instance));
        }

        #endregion

        #endregion

        #region View Methods  (to Convert Models and Execute Setup)

        #region Details View

        protected ViewResult ViewModel(TEntityType model)
        {
            var viewModel = new TViewType().Setup(model, this);
            ViewBag.Container = viewModel;
            return View(viewModel);
        }

        protected ViewResult ViewModel(string viewName, TEntityType model)
        {
            var viewModel = new TViewType().Setup(model, this);
            ViewBag.Container = viewModel;
            return View(viewName, viewModel);
        }

        protected ViewResult ViewModel(IView view, TEntityType model)
        {
            var viewModel = new TViewType().Setup(model, this);
            ViewBag.Container = viewModel;
            return base.View(view, viewModel);
        }

        #endregion

        #region Form View

        protected ViewResult ViewModel(
            TViewType viewModel,
            bool mapFromRepository = false)
        {
            ViewBag.Container = viewModel;
            return View(viewModel.Setup(this, mapFromRepository));
        }

        protected ViewResult ViewModel(
            string viewName,
            TViewType viewModel,
            bool mapFromRepository = false)
        {
            ViewBag.Container = viewModel;
            return View(viewName, viewModel.Setup(this, mapFromRepository));
        }

        protected ViewResult ViewModel(
            IView view,
            TViewType viewModel,
            bool mapFromRepository = false)
        {
            ViewBag.Container = viewModel;
            return base.View(view, viewModel.Setup(this, mapFromRepository));
        }

        #endregion

        #region List View

        protected ViewResult ViewModel(IEnumerable<TEntityType> dataModels)
        {
            return View(dataModels
                .Select(e => new TViewType().Setup(e, this)));
        }

        protected ViewResult ViewModel(string viewName, IEnumerable<TEntityType> dataModels)
        {
            return View(viewName, dataModels
                .Select(e => new TViewType().Setup(e, this)));
        }

        protected ViewResult ViewModel(IView view, IEnumerable<TEntityType> dataModels)
        {
            return base.View(view, dataModels
                .Select(e => new TViewType().Setup(e, this)));
        }

        #endregion

        #endregion


        #region Result Helpers

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="exc">The exc.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        protected ActionResult HandleEntityException(
            TViewType viewModel,
            Exception exc,
            string viewName = null)
        {
            var result = HandleException(viewModel.Setup(this, true), exc);
            result.ViewName = viewName;
            return result;
        }


        #endregion

    }
}
