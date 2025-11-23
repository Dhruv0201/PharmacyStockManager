using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyStockManager.Models
{
    public partial class Sale
    {
        [NotMapped]
        public string PaymentStatus { get; set; }
    }
}
