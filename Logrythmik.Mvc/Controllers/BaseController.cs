using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Logrythmik.Mvc.Proxies;
using Logrythmik.Mvc.ViewModels;

using Microsoft.Practices.Unity;

namespace Logrythmik.Mvc.Controllers
{
    public abstract class BaseController : Controller, IBaseController
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        protected BaseController()
        {
            this.ActionInvoker = new ControllerActionInvoker();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="actionInvoker">The action invoker.</param>
        protected BaseController(IActionInvoker actionInvoker)
        {
            this.ActionInvoker = actionInvoker;
        }

        #endregion
      
        #region Protected Members

        /// <summary>
        /// Gets the view messages.
        /// </summary>
        /// <value>The view messages.</value>
        public List<ViewMessage> ViewMessages 
        { 
            get
            {
                return this.ViewData.GetOrAdd(Constants.Messages, 
                    () => new List<ViewMessage>());
            }
        }

        protected Exception Exception;

        #endregion

        #region Properties
        
        [Dependency]
        public IDataCacheProxy DataCache { get; set; }

        #endregion
      
        #region Redirect Messaging

        /// <summary>
        /// Adds the redirect message.
        /// </summary>
        /// <param name="messageView">The message view.</param>
        public void AddRedirectMessage(ViewMessage messageView)
        {
            if (messageView == null) return;

            var messages = this.TempData.GetOrAdd(Constants.Messages, () => new List<ViewMessage>());
            messages.Add(messageView);
        }

        #endregion

        #region Messaging

        /// <summary>
        /// Adds the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void AddException(Exception exception)
        {
            if (this.Request.IsLocal)
                Exception = exception;
            else
                AddErrorMessage("An error has occurred.");
        }

        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <param name="messageView">The message view.</param>
        public void AddMessage(ViewMessage messageView)
        {
            ViewMessages.Add(messageView);
        }

        /// <summary>
        /// Adds a normal message to the viewdata.
        /// </summary>
        /// <param name="message">The message.</param>
        public void AddMessage(string message)
        {
            ViewMessages.Add(new ViewMessage(message, MessageType.Normal));
        }

        /// <summary>
        /// Adds a error message to the viewdata.
        /// </summary>
        /// <param name="message">The message.</param>
        public void AddErrorMessage(string message)
        {
            ViewMessages.Add(new ViewMessage(message, MessageType.Error));
        }

        /// <summary>
        /// Adds a warning message to the viewdata.
        /// </summary>
        /// <param name="message">The message.</param>
        public void AddWarningMessage(string message)
        {
            ViewMessages.Add(new ViewMessage(message, MessageType.Warning));
        }

        /// <summary>
        /// Adds the information message to the viewdata.
        /// </summary>
        /// <param name="message">The message.</param>
        public void AddInformationMessage(string message)
        {
            ViewMessages.Add(new ViewMessage(message, MessageType.Information));
        }

        /// <summary>
        /// Adds a success message to the viewdata.
        /// </summary>
        /// <param name="message">The message.</param>
        public void AddSuccessMessage(string message)
        {
            ViewMessages.Add(new ViewMessage(message, MessageType.Success));
        }

        #endregion

        #region Action Results



        #region Exceptions

        protected override void HandleUnknownAction(string actionName)
        {
            PageNotFound().ExecuteResult(this.ControllerContext);
        }

        protected ViewResult HandleException(object model, Exception exc)
        {
            exc.Log();
            ViewBag.Container = model;

            if(exc is MessageException)
                this.AddErrorMessage(exc.Message);
            else
                this.ModelState.AddModelError("_Form", (this.Request.IsLocal) ?
                    exc.Message : "An unknown error has occurred.");

            return View(model);
        }

        protected ViewResult HandleException(string viewName, object model, Exception exc)
        {
            exc.Log();
            ViewBag.Container = model;
            this.ModelState.AddModelError("_Form", (this.Request.IsLocal) ?
                exc.Message : "An unknown error has occurred.");

            return View(viewName, model);
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Returns a page not found result, defined in the derived class.
        /// </summary>
        /// <returns></returns>
        public abstract ActionResult PageNotFound();

        /// <summary>
        /// Redirects to the action. Use the T4Mvc Call for this.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        protected abstract RedirectToRouteResult RedirectToAction(ActionResult result);

        #endregion

        #region Script Actions

        protected JsonResult ScriptAction(string successMessage, Action action)
        {
            try
            {
                action.Invoke();

                return Json(new  { success = true, message = successMessage });
            }
            catch (MessageException exc)
            {
                return Json(new { success = false, message = exc.Message  });
            }
            catch (Exception exc)
            {
                exc.Log();
                return Json(new 
                {
                    success = false,
                    message = "An error occurred. " + ((this.Request.IsLocal) ? exc.Message : string.Empty)
                });
            }
        }

        protected JsonResult ScriptAction(Func<JsonResult> action)
        {
            try
            {
                return action.Invoke();
            }
            catch (MessageException exc)
            {
                return Json(new { success = false, message = exc.Message });
            }
            catch (Exception exc)
            {
                return Json(new 
                {
                    success = false,
                    message = (this.Request.IsLocal) ? exc.Message : "An error occurred."
                });
            }
        }

        #endregion

        #endregion

        #region Temp Data

        /// <summary>
        /// Stores data in a temp.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void StoreTemp(string key, object value)
        {
            this.TempData[key] = value;
        }

        /// <summary>
        /// Gets the temp data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T GetTemp<T>(string key) where T : class, new()
        {
            T result = default(T);
            if (this.TempData[key] != null)
                result = (T)this.TempData[key];

            //TempData only holds the bits for one request
            //so put it back in to be sure that we can get it again
            StoreTemp(key, result);
            return result;

        }

        #endregion

        protected void ClearDomainCache()
        {
            if (this.DataCache != null)
                this.DataCache.ClearGlobalCache();
        }
    }
}
