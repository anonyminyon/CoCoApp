using COCOApp.Models.ValidationAtributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace COCOApp.Models.Validations
{
    public class ExportOrderItemMetaData
    {
        [Range(1, 10000, ErrorMessage = "Số lượng phải lớn hơn không và nhỏ hơn hoặc bằng 10000.")]
        public int Volume { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng thực phải là một số không âm")]
        [RealVolumeValidation("Volume", ErrorMessage = "Số lượng thực phải nhỏ hơn hoặc bằng số lượng.")]
        public int RealVolume { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải là một giá trị dương.")]
        [WholeNumberValidation(ErrorMessage = "Giá sản phẩm phải là số nguyên")]
        public decimal ProductPrice { get; set; }

    }

}
