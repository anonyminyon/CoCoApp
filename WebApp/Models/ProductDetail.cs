using System;
using System.Collections.Generic;

namespace COCOApp.Models
{
    public partial class ProductDetail
    {
        public int ProductId { get; set; }
        public string? Description { get; set; }
        public string? AdditionalInfo { get; set; }

        public virtual Product Product { get; set; } = null!;
    }
}
