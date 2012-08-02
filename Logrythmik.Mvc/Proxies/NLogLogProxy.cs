using System;
using NLog;

namespace Logrythmik.Mvc
{
    public class NLogLogProxy : ILogProxy
    {
        private readonly Logger _Logger;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogLogProxy"/> class.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        public NLogLogProxy(string typeName)
        {
            _Logger = LogManager.GetLogger(typeName);
        }

        #endregion

        #region ILogProxy Members

        /// <summary>
        /// Gets a value indicating whether this instance is trace enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is trace enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsTraceEnabled
        {
            get { return _Logger.IsTraceEnabled; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is debug enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsDebugEnabled
        {
            get { return _Logger.IsDebugEnabled; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is warn enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is warn enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsWarnEnabled
        {
            get { return _Logger.IsWarnEnabled; }
        }

        /// <summary>
        /// Traces the specified type name.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Trace(string message)
        {
            _Logger.Trace(message);
        }

        /// <summary>
        /// Debugs the specified type name.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            _Logger.Debug(message);
        }

        /// <summary>
        /// Warns the specified type name.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            _Logger.Warn(message);
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exc">The exc.</param>
        public void LogException(Exception exc)
        {
            _Logger.LogException(LogLevel.Error, exc.Message, exc);
        }

        #endregion
    }
}