using System;
using System.Collections.Generic;
using Logrythmik.Mvc.ViewModels;

namespace Logrythmik.Mvc.Controllers
{
    public interface IBaseController
    {
        List<ViewMessage> ViewMessages { get; }

        /// <summary>
        /// Adds the redirect message.
        /// </summary>
        /// <param name="messageView">The message view.</param>
        void AddRedirectMessage(ViewMessage messageView);

        /// <summary>
        /// Adds the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void AddException(Exception exception);

        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <param name="messageView">The message view.</param>
        void AddMessage(ViewMessage messageView);

        /// <summary>
        /// Adds a normal message to the viewdata.
        /// </summary>
        /// <param name="message">The message.</param>
        void AddMessage(string message);

        /// <summary>
        /// Adds a error message to the viewdata.
        /// </summary>
        /// <param name="message">The message.</param>
        void AddErrorMessage(string message);

        /// <summary>
        /// Adds a warning message to the viewdata.
        /// </summary>
        /// <param name="message">The message.</param>
        void AddWarningMessage(string message);

        /// <summary>
        /// Adds the information message to the viewdata.
        /// </summary>
        /// <param name="message">The message.</param>
        void AddInformationMessage(string message);

        /// <summary>
        /// Adds a success message to the viewdata.
        /// </summary>
        /// <param name="message">The message.</param>
        void AddSuccessMessage(string message);
    }
}
