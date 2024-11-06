using System;
using System.Collections.Generic;

namespace COCOApp.Models
{
    public partial class InventoryManagement
    {
        public int ProductId { get; set; }
        public int RemainingVolume { get; set; }
        public int AllocatedVolume { get; set; }
        public int ShippedVolume { get; set; }

        public virtual Product Product { get; set; } = null!;
    }
}
