using System;
using System.Collections.Generic;

namespace COCOApp.Models
{
    public partial class Product
    {
        public Product()
        {
            ExportOrderItems = new HashSet<ExportOrderItem>();
            ImportOrderItems = new HashSet<ImportOrderItem>();
            ReportDetails = new HashSet<ReportDetail>();
        }

        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string ProductName { get; set; } = null!;
        public string MeasureUnit { get; set; } = null!;
        public decimal Cost { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? SellerId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual User? Seller { get; set; }
        public virtual InventoryManagement? InventoryManagement { get; set; }
        public virtual ProductDetail? ProductDetail { get; set; }
        public virtual ICollection<ExportOrderItem> ExportOrderItems { get; set; }
        public virtual ICollection<ImportOrderItem> ImportOrderItems { get; set; }
        public virtual ICollection<ReportDetail> ReportDetails { get; set; }
    }
}
