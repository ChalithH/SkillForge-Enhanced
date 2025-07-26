using System.ComponentModel.DataAnnotations;

namespace SkillForge.Api.Attributes
{
    public class OptionalUrlAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            // Allow null or empty values
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return true;
            }

            // If value is provided, validate it as a URL
            var urlAttribute = new UrlAttribute();
            return urlAttribute.IsValid(value);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field must be a valid URL when provided.";
        }
    }
}