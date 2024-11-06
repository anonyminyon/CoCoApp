using System.ComponentModel.DataAnnotations;

namespace COCOApp.Models.ValidationAtributes
{
    // Custom validation attribute to ensure RealVolume <= Volume
    public class RealVolumeValidation : ValidationAttribute
    {
        private readonly string _volumePropertyName;

        public RealVolumeValidation(string volumePropertyName)
        {
            _volumePropertyName = volumePropertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var realVolume = (int?)value;

            // Get the Volume property value
            var volumeProperty = validationContext.ObjectType.GetProperty(_volumePropertyName);
            if (volumeProperty == null)
                return new ValidationResult($"Unknown property {_volumePropertyName}");

            var volumeValue = (int?)volumeProperty.GetValue(validationContext.ObjectInstance);

            // Check if RealVolume is less than or equal to Volume
            if (realVolume.HasValue && volumeValue.HasValue && realVolume > volumeValue)
            {
                return new ValidationResult(ErrorMessage ?? $"RealVolume must be less than or equal to {volumeProperty.Name}");
            }

            return ValidationResult.Success;
        }
    }
}
