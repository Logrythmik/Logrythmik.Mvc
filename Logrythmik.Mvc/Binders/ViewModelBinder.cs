using System.Web.Mvc;
using Logrythmik.Data;
using Logrythmik.Mvc.ViewModels;

namespace Logrythmik.Mvc.Binders
{
    public class ViewModelBinder<TViewType, TEntityType, TKeyType, TDataContext> : DefaultModelBinder
        where TViewType : ViewModel<TViewType, TEntityType, TKeyType, TDataContext>, new()
        where TEntityType : class, new() where TDataContext : IDataContext, new()
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext);

            var viewModel = model as ViewModel<TViewType, TEntityType, TKeyType, TDataContext>;
            if(viewModel != null)
                viewModel.Bind(controllerContext.HttpContext.Request);

            return model;
        }
    }
}