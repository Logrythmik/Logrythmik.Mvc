using System;
using System.Linq;
using System.Linq.Expressions;

namespace Logrythmik.Data
{
    public class EntityRepository<TInstance, TDataContext> : 
            Repository<TDataContext>, 
            IEntityRepository<TInstance, TDataContext>,
            IEntityRepository
        where TInstance : class
        where TDataContext : IDataContext, new() 
    {
        #region Public Methods

        /// <summary>
        /// Lists this instance.
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TInstance> List()
        {
            return base.List<TInstance>();
        }

        /// <summary>
        /// Gets the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public virtual TInstance Get<TKeyType>(TKeyType id)
        {
            return base.Get<TInstance, TKeyType>(id);
        }

        /// <summary>
        /// Gets the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public virtual TInstance Find(Expression<Func<TInstance, bool>> selector)
        {
            return base.Find(selector);
        }

        /// <summary>
        /// Adds the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public virtual void Add(TInstance instance)
        {
            base.Add(instance);
        }

        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public virtual void Delete(TInstance instance)
        {
            base.Delete(instance);
        }

        #endregion

        #region IRepository Members

        IQueryable IEntityRepository.List()
        {
            return List<TInstance>();
        }

        object IEntityRepository.Get(int id)
        {
            return Get<TInstance, int>(id);
        }

        void IEntityRepository.Add(object instance)
        {
            base.Add((TInstance)instance);
        }

        void IEntityRepository.Delete(object instance)
        {
            base.Delete((TInstance)instance);
        }

        #endregion
    }

   
}
