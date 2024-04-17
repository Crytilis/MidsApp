using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MidsApp
{
    /// <summary>
    /// Specifies the MongoDB collection name for a class.
    /// </summary>
    /// <remarks>
    /// This attribute is intended to be placed on classes that are directly mapped to MongoDB collections.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CollectionAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the MongoDB collection associated with the class.
        /// </summary>
        public string? CollectionName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionAttribute"/> class.
        /// </summary>
        /// <param name="collectionName">The name of the MongoDB collection.</param>
        public CollectionAttribute(string? collectionName)
        {
            CollectionName = collectionName;
        }
    }

    /// <summary>
    /// Validates that the associated property is provided when submitting data via a specific endpoint.
    /// This attribute is designed to enforce the presence of required fields during submission operations,
    /// checking the request path to determine applicability.
    /// </summary>
    public class RequiredForSubmissionAttribute : ValidationAttribute
    {
        /// <summary>
        /// Checks if the property is required and provided for submission requests.
        /// </summary>
        /// <param name="value">The value of the property being validated.</param>
        /// <param name="validationContext">Context information about the validation operation.</param>
        /// <returns>A <see cref="ValidationResult"/> indicating whether the value is valid or not.</returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (validationContext.GetService(typeof(IHttpContextAccessor)) is not IHttpContextAccessor context)
            {
                return ValidationResult.Success;
            }

            var path = context.HttpContext?.Request.Path.Value;

            if (path != null && path.Contains("/submit") && value == null)
            {
                return new ValidationResult($"The {validationContext.DisplayName} field is required for submission.");
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Validates that the associated property is provided when updating data via a specific endpoint.
    /// This attribute is designed to enforce the presence of required fields during update operations,
    /// checking the request path to determine applicability.
    /// </summary>
    public class RequiredForUpdateAttribute : ValidationAttribute
    {
        /// <summary>
        /// Checks if the property is required and provided for update requests.
        /// </summary>
        /// <param name="value">The value of the property being validated.</param>
        /// <param name="validationContext">Context information about the validation operation.</param>
        /// <returns>A <see cref="ValidationResult"/> indicating whether the value is valid or not.</returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (validationContext.GetService(typeof(IHttpContextAccessor)) is not IHttpContextAccessor context)
            {
                return ValidationResult.Success;
            }

            var path = context.HttpContext?.Request.Path.Value;

            if (path != null && path.Contains("/update") && value == null)
            {
                return new ValidationResult($"The {validationContext.DisplayName} field is required for updating.");
            }

            return ValidationResult.Success;
        }
    }
}
