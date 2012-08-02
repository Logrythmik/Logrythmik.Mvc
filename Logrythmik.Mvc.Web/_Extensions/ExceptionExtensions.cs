#region Copyright

/*
 * Copyright (c) 2004-2008 IP Commerce, INC. - All Rights Reserved.
 *
 * This software and documentation is subject to and made
 * available only pursuant to the terms of an executed license
 * agreement, and may be used only in accordance with the terms
 * of said agreement. This document may not, in whole or in part,
 * be copied, photocopied, reproduced, translated, or reduced to
 * any electronic medium or machine-readable form without
 * prior consent, in writing, from IP Commerce, INC.
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
using System.Web.Mvc;
using Elmah;

namespace Ipc.Mvc
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public static void Log(this Exception exception)
        {
            try
            {
                ErrorSignal.FromCurrentContext().Raise(exception);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            {
                // We don't need to raise another error if the Context is missing
                // for unit tests.
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        /// <summary>
        /// Adds the model state errors.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="modelState">State of the model.</param>
        public static void AddModelStateErrors(this Exception exception, ModelStateDictionary modelState)
        {
            modelState.AddModelError("_FORM", exception);
        }


    }
}