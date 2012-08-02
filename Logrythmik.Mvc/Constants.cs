
namespace Logrythmik.Mvc
{
    public class Constants
    {
        public const string Message = "Message.cshtml";
        public const string Messages = "Messages";
        public const string Exception = "Exception";
        public const string ReturnUrl = "returnurl";
        public const string Error = "Error";
        public const string Domain = "Domain";

        /// <summary>
        /// These are shared views.
        /// </summary>
        public class Views
        {
            public const string Unauthorized = "Unauthorized";
            public const string Index = "Index";
            public const string Manage = "Manage";
        }

        public class Regex
        {
            public const string EmailFormat = @"[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}";
            public const string TextBox = @"^[\w\s.\(\),#]*$";
            public const string TextArea = @"[\w\s.\(\),#]*";
        }
        
    }
}
