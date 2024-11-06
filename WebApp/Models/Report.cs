using System;
using System.Collections.Generic;

namespace COCOApp.Models
{
    public partial class Report
    {
        public Report()
        {
            ReportDetails = new HashSet<ReportDetail>();
        }

        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? SellerId { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual User? Seller { get; set; }
        public virtual ICollection<ReportDetail> ReportDetails { get; set; }
    }
}
