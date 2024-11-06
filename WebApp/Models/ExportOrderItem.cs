using System;
using System.Collections.Generic;

namespace COCOApp.Models
{
    public partial class ExportOrderItem
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Volume { get; set; }
        public int RealVolume { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal Total { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? SellerId { get; set; }
        public bool Status { get; set; }

        public virtual ExportOrder? Order { get; set; } 
        public virtual Product? Product { get; set; } 
        public virtual User? Seller { get; set; }
    }
}
