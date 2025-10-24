using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;

    public string MobileNumber { get; set; } = null!;

    public string? Email { get; set; }

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual UserAccount? ModifiedByNavigation { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<PurchaseReturnHeader> PurchaseReturnHeaders { get; set; } = new List<PurchaseReturnHeader>();

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
