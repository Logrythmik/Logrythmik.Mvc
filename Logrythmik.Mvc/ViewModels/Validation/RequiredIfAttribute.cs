using System;
using System.ComponentModel.DataAnnotations;

namespace Logrythmik.Mvc.ViewModels.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        #region Private Members
        
        private const string DefaultErrorMessageFormatString = "The {0} field is required.";
        private readonly string _DependentPropertyName;
        private readonly Comparison _DependentPropertyComparison;
        private readonly object _DependentPropertyValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="comparison">The comparison.</param>
        public RequiredIfAttribute(string name, Comparison comparison)
            : this(name, comparison, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="comparison">The comparison.</param>
        /// <param name="value">The value.</param>
        public RequiredIfAttribute(string name, Comparison comparison, object value)
        {
            _DependentPropertyName = name;
            _DependentPropertyComparison = comparison;
            _DependentPropertyValue = value;

            ErrorMessage = DefaultErrorMessageFormatString;
        }

        #endregion

        #region Private Methods

        private bool ValidateDependentProperty(object value)
        {
            switch (_DependentPropertyComparison)
            {
                case Comparison.Equal:
                    return value == null ? _DependentPropertyValue == null : value.Equals(_DependentPropertyValue);
                default:
                    return value == null ? _DependentPropertyValue != null : !value.Equals(_DependentPropertyValue);
            }
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
            var property = validationContext.ObjectInstance.GetType().GetProperty(_DependentPropertyName);
            var depValue = property.GetValue(validationContext.ObjectInstance, null);
            if (ValidateDependentProperty(depValue) && value == null)
                return new ValidationResult(string.Format(ErrorMessageString, validationContext.DisplayName));

            return ValidationResult.Success;
        }

        #endregion
    }

    public enum Comparison
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan
    }
}