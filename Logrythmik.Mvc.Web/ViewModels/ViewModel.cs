using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using AutoMapper;
using Logrythmik.Data;
using Logrythmik.Mvc.Controllers;

namespace Logrythmik.Mvc.ViewModels
{
    public abstract class ViewModel<TViewType, TEntityType, TKeyType, TDataContext> : IValidatableObject
        where TEntityType : class, new()
        where TViewType : ViewModel<TViewType, TEntityType, TKeyType, TDataContext>, new()
        where TDataContext : IDataContext, new()
    {

        #region Static Members
        // ReSharper disable StaticFieldInGenericType

        /// <summary>
        /// Build on this expression to define the mappings from the data model
        /// to the edit view model.
        /// </summary>
        protected static IMappingExpression<TEntityType, TViewType> ModelToViewMap;


        /// <summary>
        /// Build on this expression to define the mappings from the view model
        /// to the data model.
        /// </summary>
        protected static IMappingExpression<TViewType, TEntityType> ViewToModelMap;

        /// <summary>
        /// This is where any external lists or data to be added to the view can be defined. 
        /// </summary>
        protected static Action<EntityController<TViewType, TEntityType, TKeyType, TDataContext>, TViewType> ExternalDataSetup;

        /// <summary>
        /// For every property that came from the LinqModel -- we need to define a way to populate that
        /// without the model using the repository. This should be setup here.
        /// </summary>
        protected static Action<IRepository<TDataContext>, TViewType> RepositoryToViewMapping;


        /// <summary>
        /// This is where any external lists or data to be added to the view can be defined. 
        /// </summary>
        protected static Action<HttpRequestBase, TViewType> RequestBinding;

        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Initializes the <see cref="ViewModel&lt;TViewType, TEntityType, TKeyType, TDataContext&gt;"/> class.
        /// </summary>
        static ViewModel()
        {
            // VIEW TO MODEL

            // Always create a basic map on view model creation, then expose
            // the mapping expression for extension in the consuming classes
            ViewToModelMap = Mapper.CreateMap<TViewType, TEntityType>();

            // Always ignore relationship and identity properties when mapping 
            // from view to model.
            var modelType = typeof(TEntityType);
            modelType.GetProperties()
                .Where(propertyInfo =>
                    propertyInfo.IsAssociation() || propertyInfo.IsPrimaryKey())
                .ForEach(pi =>
                    ViewToModelMap.ForMember(pi.Name, m => m.Ignore()));

            // Always ignore Created and Updated
            if (modelType.IsA<ICreated>())
                ViewToModelMap.ForMember("Created", m => m.Ignore());

            if (modelType.IsA<IUpdated>())
                ViewToModelMap.ForMember("Updated", m => m.Ignore());

            // MODEL TO VIEW

            ModelToViewMap = Mapper.CreateMap<TEntityType, TViewType>();
        }

        #endregion

        protected bool MappedFromModel;
        protected bool MappedFromRequest;
        protected bool MappedFromRepository;

        [XmlIgnore]
        public TEntityType Model { get; private set; }


        #region View Mapping Methods

        /// <summary>
        /// Maps the view from repository. Use this when the entity is disconnected from data-context.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <returns></returns>
        public virtual TViewType MapFromRepository(
            IRepository<TDataContext> repository)
        {
            if (RepositoryToViewMapping != null)
                RepositoryToViewMapping(repository, this as TViewType);

            MappedFromRepository = true;

            return this as TViewType;
        }

        /// <summary>
        /// Maps this view from model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public virtual TViewType MapFromModel(TEntityType model)
        {
            Mapper.Map(model, this as TViewType);

            MappedFromModel = true;

            this.Model = model;

            return this as TViewType;
        }

        #endregion

        #region Setup Helpers

        /// <summary>
        /// Setups the external data for use in the UI.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns></returns>
        public virtual TViewType SetupExternalData(EntityController<TViewType, TEntityType, TKeyType, TDataContext> controller)
        {
            if (ExternalDataSetup != null)
                ExternalDataSetup(controller, this as TViewType);

            return this as TViewType;
        }

        /// <summary>
        /// Setups the view from model, and also calls SetupExternalData
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="controller">The controller.</param>
        /// <returns></returns>
        public virtual TViewType Setup(
            TEntityType model,
            EntityController<TViewType, TEntityType, TKeyType, TDataContext> controller)
        {
            MapFromModel(model)
                .SetupExternalData(controller);

            return this as TViewType;
        }

        /// <summary>
        /// Setups the view from repository, and also calls SetupExternalData
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="mapFromRepository">if set to <c>true</c> [map from repository].</param>
        /// <returns></returns>
        public virtual TViewType Setup(
            EntityController<TViewType, TEntityType, TKeyType, TDataContext> controller,
            bool mapFromRepository = false)
        {
            SetupExternalData(controller);

            if (mapFromRepository)
                MapFromRepository(controller.Repository);

            return this as TViewType;
        }

        /// <summary>
        /// Maps the view, from request as part of binding. Use this to grab things that binding couldn't get.
        /// </summary>
        /// <param name="request">The request.</param>
        public virtual TViewType Bind(HttpRequestBase request)
        {
            if (RequestBinding != null)
                RequestBinding(request, this as TViewType);

            MappedFromRequest = true;

            return this as TViewType;
        }

        #endregion

        #region Persistence Methods

        /// <summary>
        /// Updates the model, using the data from the view
        /// </summary>
        /// <param name="model">The model.</param>
        public virtual void UpdateModel(TEntityType model)
        {
            Mapper.Map(this as TViewType, model);
        }

        /// <summary>
        /// Creates a new model, using the data from the view
        /// </summary>
        /// <returns></returns>
        public virtual TEntityType CreateModel()
        {
            return (TEntityType)Mapper.Map(this as TViewType, typeof(TViewType), typeof(TEntityType));
        }

        #endregion


        #region IValidatableObject Members

        /// <summary>
        /// Determines whether the specified object is valid.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>
        /// A collection that holds failed-validation information.
        /// </returns>
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult> { null };
        }

        #endregion

    }
}
