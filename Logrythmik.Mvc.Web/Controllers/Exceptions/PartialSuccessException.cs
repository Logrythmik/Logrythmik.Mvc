using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logrythmik.Mvc.Controllers.Exceptions
{
    /// <summary>
    /// Throw the PartialSuccessException to add a warning redirect message from the
    /// controller, but otherwise allow the controller action to "succeed".
    /// </summary>
    public class PartialSuccessException : MessageException
    {
        public PartialSuccessException(string message)
            : base(message) {}

        public PartialSuccessException(string message, Exception innerException)
            : base(message, innerException) {}
    }
}
