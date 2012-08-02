using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Logrythmik.Mvc.Controllers
{
    public class ControllerActionInvoker: System.Web.Mvc.ControllerActionInvoker
    {
        protected override ActionResult InvokeActionMethod(ControllerContext controllerContext, ActionDescriptor actionDescriptor, System.Collections.Generic.IDictionary<string, object> parameters)
        {
            try
            {
                return base.InvokeActionMethod(controllerContext, actionDescriptor, parameters);
            }
            catch (Exception exc)
            {
                if(controllerContext.HttpContext.Request.IsLocal)
                    throw;

                var ipcController = controllerContext.Controller as BaseController;
                if (ipcController != null)
                    return ipcController.PageNotFound();
                
                throw new HttpException((int)HttpStatusCode.NotFound, "File Not Found.", exc);
            }
        }
    }


}