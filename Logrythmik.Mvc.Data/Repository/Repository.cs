using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace Logrythmik.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class Repository<TDataContext> : IRepository<TDataContext>
        where TDataContext: IDataContext, new()
    {
        public IDataContext DataContext { get; set; }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository&lt;TDataContext&gt;"/> class.
        /// </summary>
        public Repository()
        {
            DataContext = new TDataContext();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Lists the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public virtual IQueryable List(Type type)
        {
            return DataContext.GetTable(type).AsQueryable();
        }

        /// <summary>
        /// Lists this entities by type.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual IQueryable<TEntity> List<TEntity>() 
            where TEntity : class
        {
            var results = DataContext.GetQueryable<TEntity>();

            // Auto sort those types that declare they are sortable.
            if (typeof(ISortable).IsAssignableFrom(typeof(TEntity)))
                results = results
                        .OrderBy(t => ((ISortable)t).SortOrder);

            // Automatically hide inactive IActive entities.
            if (typeof(IActive).IsAssignableFrom(typeof(TEntity)))
                results = results.Cast<IActive>()
                        .Where(e => e.IsActive)
                        .Cast<TEntity>();

            return results;
        }

        /// <summary>
        /// Gets the instance by the specified id.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKeyType">The type of the ID type.</typeparam>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public virtual TEntity Get<TEntity, TKeyType>(TKeyType id)
            where TEntity : class
        {
            return DataContext.GetEntity<TEntity, TKeyType>(id);
        }

        /// <summary>
        /// Gets an instance by the specified selector.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public virtual TEntity Find<TEntity>(Expression<Func<TEntity, bool>> selector)
            where TEntity : class
        {
            return DataContext.GetQueryable<TEntity>().SingleOrDefault(selector);
        }

        /// <summary>
        /// Adds the specified instance.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="instance">The instance.</param>
        public virtual void Add<TEntity>(TEntity instance) 
            where TEntity : class
        {
            DataContext.GetEditable<TEntity>().InsertOnSubmit(instance);
        }

        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="instance">The instance.</param>
        public virtual void Delete<TEntity>(TEntity instance) 
            where TEntity : class
        {
            ValidateDeletion(instance);

            if (instance is IActive)
                ((IActive) instance).IsActive = false;
            else
                DataContext.GetEditable<TEntity>().DeleteOnSubmit(instance);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public int ExecuteCommand(string command)
        {
            return DataContext.ExecuteCommand(command);
        }

        /// <summary>
        /// Attaches the specified entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">The entity.</param>
        public void Attach<TEntity>(TEntity entity)
        {
            DataContext.GetTable(typeof(TEntity)).Attach(entity);
        }

        /// <summary>
        /// Submits the changes.
        /// </summary>
        public virtual void SubmitChanges()
        {
            var changes = DataContext.GetChangeSet();

            changes.Inserts.Where(e => e is ICreated)
                .ForEach(e => ((ICreated)e).Created = DateTime.UtcNow);

            changes.Updates.Where(e => e is IUpdated)
                .ForEach(e => ((IUpdated)e).Updated = DateTime.UtcNow);

            DataContext.SubmitChanges();
        }

        /// <summary>
        /// Gets the modified members.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public IEnumerable<ModifiedMemberInfo> GetModifiedMembers<TEntity>(TEntity entity)
        {
            return this.DataContext.GetTable(typeof(TEntity)).GetModifiedMembers(entity);
        }

        #endregion

        #region Private Helpers

        private static void ValidateDeletion(object instance)
        {
            if (instance == null)
                throw new NullReferenceException("No instance found to delete.");

            if (instance is IUsable)
            {
                if (((IUsable)instance).IsInUse)
                    throw new ConstraintException(string.Format(
                        "You cannot delete this {0} while it is in use.",
                        instance.GetType().DisplayName()));
            }
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                DataContext.Dispose();
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
                // DO Nothing.
            }
        }

        #endregion
    }
}
