using System.ComponentModel.DataAnnotations;

namespace COCOApp.Models.ValidationAtributes
{
    public class WholeNumberValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is decimal decimalValue)
            {
                // Check if the value is a whole number (no decimal places)
                if (decimalValue == Math.Floor(decimalValue))
                {
                    return ValidationResult.Success!;
                }
                else
                {
                    return new ValidationResult("Giá sản phẩm phải là một số nguyên");
                }
            }

            return new ValidationResult("Giá trị không hợp lệ");
        }
    }
}
