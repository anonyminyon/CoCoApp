using System;
using System.Collections.Generic;

namespace COCOApp.Models
{
    public partial class Customer
    {
        public Customer()
        {
            ExportOrders = new HashSet<ExportOrder>();
            Reports = new HashSet<Report>();
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
        public virtual ICollection<ExportOrder> ExportOrders { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
