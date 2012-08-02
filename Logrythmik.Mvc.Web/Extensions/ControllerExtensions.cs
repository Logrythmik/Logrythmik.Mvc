using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Logrythmik.Mvc.ViewModels;

namespace Logrythmik.Mvc
{
    public static class ControllerExtensions
    {
        #region Excel Results
        
        public static ExcelResult Excel(this Controller controller,
            IQueryable rows,
            string fileName)
        {
            return new ExcelResult(rows, fileName, null, null, null, null);
        }

        public static ExcelResult Excel(this Controller controller,
            IQueryable rows,
            string fileName,
            string[] headers)
        {
            return new ExcelResult(rows, fileName, headers, null, null, null);
        }

        public static ExcelResult Excel(this Controller controller,
            IQueryable rows,
            string fileName,
            string[] headers,
            TableStyle tableStyle,
            TableItemStyle headerStyle,
            TableItemStyle itemStyle)
        {
            return new ExcelResult(rows, fileName, headers, tableStyle, headerStyle, itemStyle);
        }

        #endregion

        public static ViewResultBase WithData<TDataType>(this ViewResultBase viewResultBase, TDataType data)
        {
            if (Equals(data, default(TDataType)))
                return viewResultBase;
            if (viewResultBase.ViewData.ContainsKey(typeof(TDataType).ToString()))
                viewResultBase.ViewData[typeof(TDataType).ToString()] = data;
            else
                viewResultBase.ViewData.Add(typeof(TDataType).ToString(), data);
            return viewResultBase;
        }

        public static ViewResultBase WithData(this ViewResultBase viewResultBase, Type entityType, object data)
        {
            if (viewResultBase.ViewData.ContainsKey(entityType.ToString()))
                viewResultBase.ViewData[entityType.ToString()] = data;
            else
                viewResultBase.ViewData.Add(entityType.ToString(), data);
            return viewResultBase;
        }

        /// <summary>
        /// Stores data in a temp.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void StoreTempData(this Controller controller, string key, object value)
        {
            controller.TempData[key] = value;
        }

        /// <summary>
        /// Gets the temp data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller">The controller.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T GetTempData<T>(this Controller controller, string key) where T : class, new()
        {
            if (controller.TempData[key] != null && controller.TempData[key] is T)
                return (T)controller.TempData[key];

            return default(T);
        }

        #region Redirect Messaging

        private static List<ViewMessage> GetRedirectMessages(this Controller controller)
        {
            return controller.TempData.GetOrAdd(Constants.Messages,
                () => new List<ViewMessage>());
        }

        /// <summary>
        /// Adds the redirect message.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="messageView">The message view.</param>
        public static void AddRedirectMessage(this Controller controller, ViewMessage messageView)
        {
            if (messageView == null) return;

            controller.GetRedirectMessages().Add(messageView);
        }

        /// <summary>
        /// Adds a normal message to the viewdata.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="message">The message.</param>
        public static void AddRedirectMessage(this Controller controller, string message)
        {
            controller.GetRedirectMessages().Add(new ViewMessage(message, MessageType.Normal));
        }

        /// <summary>
        /// Adds a error message to the viewdata.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="message">The message.</param>
        public static void AddRedirectErrorMessage(this Controller controller, string message)
        {
            controller.GetRedirectMessages().Add(new ViewMessage(message, MessageType.Error));
        }

        /// <summary>
        /// Adds a warning message to the viewdata.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="message">The message.</param>
        public static void AddRedirectWarningMessage(this Controller controller, string message)
        {
            controller.GetRedirectMessages().Add(new ViewMessage(message, MessageType.Warning));
        }

        /// <summary>
        /// Adds the information message to the viewdata.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="message">The message.</param>
        public static void AddRedirectInformationMessage(this Controller controller, string message)
        {
            controller.GetRedirectMessages().Add(new ViewMessage(message, MessageType.Information));
        }

        /// <summary>
        /// Adds a success message to the viewdata.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="message">The message.</param>
        public static void AddRedirectSuccessMessage(this Controller controller, string message)
        {
            controller.GetRedirectMessages().Add(new ViewMessage(message, MessageType.Success));
        }

        #endregion

        #region View Messaging

        private static List<ViewMessage> GetViewMessages(this Controller controller)
        {
            return controller.ViewData.GetOrAdd(Constants.Messages,
                () => new List<ViewMessage>());
        }

        /// <summary>
        /// Adds the exception.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="exception">The exception.</param>
        public static void AddException(this Controller controller, Exception exception)
        {
            if (controller.Request.IsLocal)
                AddException(controller, exception);
            else
                AddErrorMessage(controller, "An error has occurred.");
        }

        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="messageView">The message view.</param>
        public static void AddMessage(this Controller controller, ViewMessage messageView)
        {
            controller.GetViewMessages().Add(messageView);
        }

        /// <summary>
        /// Adds a normal message to the viewdata.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="message">The message.</param>
        public static void AddMessage(this Controller controller, string message)
        {
            controller.GetViewMessages().Add(new ViewMessage(message, MessageType.Normal));
        }

        /// <summary>
        /// Adds a error message to the viewdata.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="message">The message.</param>
        public static void AddErrorMessage(this Controller controller, string message)
        {
            controller.GetViewMessages().Add(new ViewMessage(message, MessageType.Error));
        }

        /// <summary>
        /// Adds a warning message to the viewdata.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="message">The message.</param>
        public static void AddWarningMessage(this Controller controller, string message)
        {
            controller.GetViewMessages().Add(new ViewMessage(message, MessageType.Warning));
        }

        /// <summary>
        /// Adds the information message to the viewdata.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="message">The message.</param>
        public static void AddInformationMessage(this Controller controller, string message)
        {
            controller.GetViewMessages().Add(new ViewMessage(message, MessageType.Information));
        }

        /// <summary>
        /// Adds a success message to the viewdata.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="message">The message.</param>
        public static void AddSuccessMessage(this Controller controller, string message)
        {
            controller.GetViewMessages().Add(new ViewMessage(message, MessageType.Success));
        }

        #endregion
        
    }
}