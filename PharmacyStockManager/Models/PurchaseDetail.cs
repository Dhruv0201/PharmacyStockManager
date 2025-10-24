using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class PurchaseDetail
{
    public int PurchaseDetailId { get; set; }

    public int? PurchaseId { get; set; }

    public int? ProductId { get; set; }

    public string? BatchNumber { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual UserAccount? ModifiedByNavigation { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Purchase? Purchase { get; set; }
}
