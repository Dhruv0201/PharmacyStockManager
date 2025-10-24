using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class PurchaseReturnHeader
{
    public int PurchaseReturnId { get; set; }

    public int? PurchaseId { get; set; }

    public int? SupplierId { get; set; }

    public DateTime? ReturnDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? ReturnedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? Reason { get; set; }

    public virtual UserAccount? ModifiedByNavigation { get; set; }

    public virtual Purchase? Purchase { get; set; }

    public virtual ICollection<PurchaseReturnDetail> PurchaseReturnDetails { get; set; } = new List<PurchaseReturnDetail>();

    public virtual UserAccount? ReturnedByNavigation { get; set; }

    public virtual Supplier? Supplier { get; set; }
}
