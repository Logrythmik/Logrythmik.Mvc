using System.ComponentModel.DataAnnotations;
namespace Logrythmik.Mvc.ViewModels.Attributes
{
    public class EmailAttribute : RegularExpressionAttribute
    {
        public EmailAttribute()
            : base(@"^\S+@\S+$") // not a valid email, but better than a typo
        {}
    }
}