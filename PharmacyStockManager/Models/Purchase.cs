using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class Purchase
{
    public int PurchaseId { get; set; }

    public int? SupplierId { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public string? InvoiceImagePath { get; set; }

    public decimal TotalAmount { get; set; }

    public int? PurchasedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual UserAccount? ModifiedByNavigation { get; set; }

    public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

    public virtual ICollection<PurchaseReturnHeader> PurchaseReturnHeaders { get; set; } = new List<PurchaseReturnHeader>();

    public virtual UserAccount? PurchasedByNavigation { get; set; }

    public virtual Supplier? Supplier { get; set; }
}
