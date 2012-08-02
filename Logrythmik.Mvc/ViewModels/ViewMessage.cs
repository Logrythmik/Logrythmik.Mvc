#region Copyright
/*
 * Copyright (c) 2005-2009 Logrythmik Consulting, LLC. - All Rights Reserved.
 *
 * This software and documentation is subject to and made
 * available only pursuant to the terms of an executed license
 * agreement, and may be used only in accordance with the terms
 * of said agreement. This document may not, in whole or in part,
 * be copied, photocopied, reproduced, translated, or reduced to
 * any electronic medium or machine-readable form without
 * prior consent, in writing, from Logrythmik Consulting, LLC.
 *
 * Use, duplication or disclosure by the U.S. Government is subject
 * to restrictions set forth in an executed license agreement
 * and in subparagraph (c)(1) of the Commercial Computer
 * Software-Restricted Rights Clause at FAR 52.227-19; subparagraph
 * (c)(1)(ii) of the Rights in Technical Data and Computer Software
 * clause at DFARS 252.227-7013, subparagraph (d) of the Commercial
 * Computer Software--Licensing clause at NASA FAR supplement
 * 16-52.227-86; or their equivalent.
 *
 * Information in this document is subject to change without notice
 * and does not represent a commitment on the part of IP Commerce.
 *
 */
#endregion

using System;

namespace Logrythmik.Mvc.ViewModels
{
    public class ViewMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public MessageType Type { get; set; }

        public bool UseNotification { get; set; }

        public ViewMessage AsNotification()
        {
            this.UseNotification = true;
            return this;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { 
            get { return _Message
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty)
                    .Replace("\"","'");  }
            set { _Message = value; }
        }
        private string _Message;

        /// <summary>
        /// Gets or sets the link text.
        /// </summary>
        /// <value>The link text.</value>
        public string LinkText { get; set; }

        /// <summary>
        /// Gets or sets the link URL.
        /// </summary>
        /// <value>The link URL.</value>
        public string LinkUrl { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has link.
        /// </summary>
        /// <value><c>true</c> if this instance has link; otherwise, <c>false</c>.</value>
        public bool HasLink
        {
            get { return !(string.IsNullOrEmpty(this.LinkText) || string.IsNullOrEmpty(this.LinkUrl)); }
        }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        /// <value>The stack trace.</value>
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show close button].
        /// </summary>
        /// <value><c>true</c> if [show close button]; otherwise, <c>false</c>.</value>
        public bool ShowCloseButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [auto hide].
        /// </summary>
        /// <value><c>true</c> if [auto hide]; otherwise, <c>false</c>.</value>
        public bool AutoHide { get; set; }

        /// <summary>
        /// Gets or sets the error list.
        /// </summary>
        /// <value>The error list.</value>
        public string ErrorList { get; set; }

        /// <summary>
        /// Gets the display type.
        /// </summary>
        /// <value>The display type.</value>
        public string DisplayType
        {
            get
            {
                string suffix = (this.AutoHide) ? "T fade" : "T";
                return this.Type.ToString().ToLower() + suffix;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMessage"/> class.
        /// </summary>
        public ViewMessage()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        public ViewMessage(string message, MessageType type)
        {
            this.Type = type;
            this.Message = message;
            this.ShowCloseButton = true;
        }

        public ViewMessage(string message, MessageType type, bool autoHide)
        {
            this.Type = type;
            this.Message = message;
            this.AutoHide = autoHide;
            this.ShowCloseButton = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMessage"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        /// <param name="linkText">The link text.</param>
        /// <param name="linkUrl">The link URL.</param>
        public ViewMessage(string message, MessageType type, string linkText, string linkUrl)
        {
            this.Type = type;
            this.Message = message;
            this.LinkText = linkText;
            this.LinkUrl = linkUrl;
            this.ShowCloseButton = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMessage"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        /// <param name="linkText">The link text.</param>
        /// <param name="linkUrl">The link URL.</param>
        /// <param name="autoHide">if set to <c>true</c> [auto hide].</param>
        public ViewMessage(string message, MessageType type, string linkText, string linkUrl, bool autoHide)
        {
            this.Type = type;
            this.Message = message;
            this.LinkText = linkText;
            this.LinkUrl = linkUrl;
            this.ShowCloseButton = true;
            this.AutoHide = autoHide;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorList">The error list.</param>
        /// <param name="type">The type.</param>
        public ViewMessage(string message, MessageType type, string errorList)
        {
            this.Type = type;
            this.Message = message;
            this.ErrorList = errorList;
            this.ShowCloseButton = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMessage"/> class.
        /// </summary>
        /// <param name="exc">The exc.</param>
        public ViewMessage(Exception exc)
        {
            this.Type = MessageType.Error;
            this.Message = exc.Message;
            this.StackTrace = exc.StackTrace;
            this.ShowCloseButton = false;
        }

        #endregion

    }


}
