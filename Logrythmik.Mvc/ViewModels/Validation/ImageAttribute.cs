using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Drawing;
using System.IO;

namespace Logrythmik.Mvc.ViewModels.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ImageAttribute : ValidationAttribute
    {
        #region Private Members
        
        private const string NotAnImageErrorMessageFormatString = "The {0} field must be a supported image type, and within the given size range.";

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the width of the max.
        /// </summary>
        /// <value>The width of the max.</value>
        public double? MaxWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the max.
        /// </summary>
        /// <value>The height of the max.</value>
        public double? MaxHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the min.
        /// </summary>
        /// <value>The width of the min.</value>
        public double? MinWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the min.
        /// </summary>
        /// <value>The height of the min.</value>
        public double? MinHeight { get; set; }

        #endregion

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageAttribute"/> class.
        /// </summary>
        public ImageAttribute()
        {
            ErrorMessage = NotAnImageErrorMessageFormatString;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Determines whether the specified value is valid.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            // we're not checking "required", only that a non-null value is an image
            if (value == null)
                return ValidationResult.Success;

            var binary = value as Binary;
            if (binary == null)
                return new ValidationResult(String.Format("The field {0} is not a Linq.Binary field!", context.DisplayName));

            try
            {
                var img = Image.FromStream(new MemoryStream(binary.ToArray()));
                if (img.Width > MaxWidth || img.Width < MinWidth || img.Height > MaxHeight || img.Height < MinHeight)
                    return new ValidationResult(String.Format(ErrorMessage, context.DisplayName));
            }
            catch (Exception)
            {
                return new ValidationResult(String.Format(ErrorMessage, context.DisplayName));
            }

            return ValidationResult.Success;
        }

        #endregion
    }
}
