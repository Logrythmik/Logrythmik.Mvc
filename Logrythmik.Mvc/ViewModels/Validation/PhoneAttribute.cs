using System.ComponentModel.DataAnnotations;

namespace Logrythmik.Mvc.ViewModels.Attributes
{
    public class PhoneAttribute : RegularExpressionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneAttribute"/> class.
        /// </summary>
        public PhoneAttribute() 
            : base(@"\d+") // most default bad ever
        {
        }
    }
}