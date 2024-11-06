using COCOApp.Models.ValidationAtributes;
using System.ComponentModel.DataAnnotations;

namespace COCOApp.Models.Validations
{
    public class ProductMetaData
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(100, ErrorMessage = "Độ dài tên sản phẩm không được vượt quá 100 ký tự")]
        [MinLength(3, ErrorMessage = "Phải có ít nhất 3 ký tự")]
        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage = "Đơn vị đo lường là bắt buộc")]
        [StringLength(50, ErrorMessage = "Độ dài đơn vị đo lường không được vượt quá 50 ký tự")]
        [MinLength(3, ErrorMessage = "Phải có ít nhất 3 ký tự")]
        public string MeasureUnit { get; set; } = null!;

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
        [WholeNumberValidation(ErrorMessage = "Giá sản phẩm phải là số nguyên")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public bool Status { get; set; }
    }
}
