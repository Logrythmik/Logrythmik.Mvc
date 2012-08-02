#region Copyright

/*
 * Copyright (c) 2004-2008 Logrythmik Consulting, LLC. - All Rights Reserved.
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

using System.Net.Mail;

namespace Logrythmik.Mvc.Proxies
{
    public class SmtpMailProxy : IMailProxy
    {
        #region Dependencies

        private readonly SmtpClient _MailClient;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpMailProxy"/> class.
        /// </summary>
        public SmtpMailProxy()
        {
            _MailClient = new SmtpClient();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="mailMessage">The mail message.</param>
        public void SendEmail(MailMessage mailMessage)
        {
            _MailClient.Send(mailMessage);
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="recipients">The recipients.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        public void SendEmail(string from, string recipients, string subject, string body)
        {
            _MailClient.Send(from, recipients, subject, body);
        }

        #endregion

        
    }
}