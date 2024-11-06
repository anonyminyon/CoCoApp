using System.ComponentModel.DataAnnotations;

namespace COCOApp.ViewModels
{
    public class MultiImportOrderViewModel
    {
        [Required]
        public List<ImportOrderEntry> ImportOrders { get; set; } = new List<ImportOrderEntry>();

        public class ImportOrderEntry
        {
            public int SupplierId { get; set; }

            [Required]
            public DateTime OrderDate { get; set; }

            public int ProductId { get; set; }
            public int ProductVolume { get; set; }
        }
    }
}
