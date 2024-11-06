using System;
using System.Collections.Generic;

namespace COCOApp.Models
{
    public partial class SellerDetail
    {
        public int UserId { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessAddress { get; set; }
        public byte[]? ImageData { get; set; }

        public virtual User? User { get; set; } 
    }
}
