using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Logrythmik.Mvc.Proxies;
using Logrythmik.Mvc.ViewModels;

using Microsoft.Practices.Unity;

namespace Logrythmik.Mvc.Controllers
{
    public abstract class BaseController : Controller
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
     
        #region Properties
        
        [Dependency]
        public IDataCacheProxy DataCache { get; set; }

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



        #endregion

        protected void ClearDomainCache()
        {
            if (this.DataCache != null)
                this.DataCache.ClearGlobalCache();
        }
    }
}
