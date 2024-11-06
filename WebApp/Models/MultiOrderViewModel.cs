using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace COCOApp.ViewModels
{
    public class MultiOrderViewModel
    {
        [Required]
        public List<OrderEntry> Orders { get; set; } = new List<OrderEntry>();

        public class OrderEntry
        {
            public int CustomerId { get; set; }

            [Required]
            public DateTime OrderDate { get; set; }

            public int ProductId { get; set; }
            public int ProductVolume { get; set; }
        }
    }
}
