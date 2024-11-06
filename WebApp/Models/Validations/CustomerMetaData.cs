using System.ComponentModel.DataAnnotations;

namespace COCOApp.Models.Validations
{
    public class CustomerMetaData
    {
        [Required(ErrorMessage = "Tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Độ dài tên không được vượt quá 100 ký tự")]
        [MinLength(3, ErrorMessage = "Phải có ít nhất 3 ký tự")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(200, ErrorMessage = "Độ dài địa chỉ không được vượt quá 200 ký tự")]
        [MinLength(3, ErrorMessage = "Phải có ít nhất 3 ký tự")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Độ dài ghi chú không được vượt quá 500 ký tự")]
        public string? Note { get; set; }
    }
}
