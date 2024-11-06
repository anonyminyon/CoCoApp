using System.ComponentModel.DataAnnotations;

namespace COCOApp.Models.Validations
{
    public class UserDetailMetaData
    {
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Độ dài họ và tên không được vượt quá 100 ký tự")]
        [MinLength(3, ErrorMessage = "Họ và tên phải có ít nhất 3 ký tự")]
        public string Fullname { get; set; } = null!;

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(200, ErrorMessage = "Độ dài địa chỉ không được vượt quá 200 ký tự")]
        [MinLength(3, ErrorMessage = "Phải có ít nhất 3 ký tự")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        public DateTime Dob { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc")]
        public bool Gender { get; set; }
    }
}
