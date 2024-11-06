using System;
using System.Collections.Generic;

namespace COCOApp.Models
{
    public partial class ReportDetail
    {
        public int ReportId { get; set; }
        public int ProductId { get; set; }
        public int Volume { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? OrderDate { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual Report Report { get; set; } = null!;
    }
}
