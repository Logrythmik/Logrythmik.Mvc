using System;

namespace Logrythmik.Mvc.Controllers
{
    public class MessageException : Exception
    {
        public MessageException(string message) : base(message)
        { }

        public MessageException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}