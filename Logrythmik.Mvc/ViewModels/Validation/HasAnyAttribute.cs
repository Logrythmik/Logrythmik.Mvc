using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Logrythmik.Mvc.ViewModels.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HasAnyAttribute : ValidationAttribute
    {
        #region Private Members

        private const string DefaultErrorMessageFormatString = "The {0} field must have one or more items selected.";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HasAnyAttribute"/> class.
        /// </summary>
        public HasAnyAttribute()
        {
            ErrorMessage = DefaultErrorMessageFormatString;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult"/> class.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var list = value as IEnumerable;
            if (list == null)
                return new ValidationResult(string.Format("The field {0} is not Enumerable!", validationContext.DisplayName));

            if (!list.GetEnumerator().MoveNext())
                return new ValidationResult(string.Format(ErrorMessageString, validationContext.DisplayName));

            return ValidationResult.Success;
        }

        #endregion
    }
}