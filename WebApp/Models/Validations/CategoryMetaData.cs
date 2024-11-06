using System.ComponentModel.DataAnnotations;

namespace COCOApp.Models.Validations
{
    public class CategoryMetaData
    {
        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(100, ErrorMessage = "Độ dài tên danh mục không được vượt quá 100 ký tự")]
        [MinLength(3, ErrorMessage = "Phải có ít nhất 3 ký tự")]
        public string CategoryName { get; set; } = null!;

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public bool Status { get; set; }
    }
}
