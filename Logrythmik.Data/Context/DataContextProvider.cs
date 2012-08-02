using System;
using System.Data.Linq;

namespace Logrythmik.Data
{
    public class DataContextProvider<TDataContextInterface, TDataContext> : IDataContextProvider<TDataContextInterface, TDataContext>
        where TDataContextInterface : class, IDisposable
        where TDataContext : IDataContext, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextProvider&lt;TDataContextInterface, TDataContext&gt;"/> class.
        /// </summary>
        public DataContextProvider()
        {
            Context = new TDataContext() as TDataContextInterface;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextProvider&lt;TDataContextInterface, TDataContext&gt;"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        internal DataContextProvider(TDataContextInterface dataContext)
        {
            Context = dataContext;
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public TDataContextInterface Context { get; set; }

        /// <summary>
        /// Gets the new context.
        /// </summary>
        /// <param name="dataLoadOptions">The data load options.</param>
        /// <returns></returns>
        public TDataContextInterface GetNewContext(DataLoadOptions dataLoadOptions)
        {
            var context = new TDataContext {LoadOptions = dataLoadOptions};
            return context as TDataContextInterface;
        }
    }
}
