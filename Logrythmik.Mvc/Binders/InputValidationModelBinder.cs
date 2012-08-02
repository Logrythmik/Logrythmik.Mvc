using System.Linq;
using System.Web.Mvc;


namespace Logrythmik.Mvc.Binders
{
    public class InputValidationModelBinder : DefaultModelBinder
    {
        protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var modelState = controllerContext.Controller.ViewData.ModelState;
            var valueProvider = controllerContext.Controller.ValueProvider;

            var keysWithNoIncomingValue = modelState.Keys.Where(x => !valueProvider.ContainsPrefix(x));
            foreach (var key in keysWithNoIncomingValue)
                modelState[key].Errors.Clear();
        }
    }
}
