using System;
using System.Collections.Generic;

namespace COCOApp.Models
{
    public partial class ImportOrder
    {
        public ImportOrder()
        {
            ImportOrderItems = new HashSet<ImportOrderItem>();
        }

        public int Id { get; set; }
        public int? SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public bool Complete { get; set; }
        public decimal OrderTotal { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? SellerId { get; set; }

        public virtual User? Seller { get; set; }
        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<ImportOrderItem> ImportOrderItems { get; set; }
    }
}
