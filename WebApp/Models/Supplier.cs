using System;
using System.Collections.Generic;

namespace COCOApp.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            ImportOrders = new HashSet<ImportOrder>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Note { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? SellerId { get; set; }

        public virtual User? Seller { get; set; }
        public virtual ICollection<ImportOrder> ImportOrders { get; set; }
    }
}
