using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class PriceHistory
{
    public int PriceHistoryId { get; set; }

    public int? ProductId { get; set; }

    public decimal? OldPurchasePrice { get; set; }

    public decimal? OldSellingPrice { get; set; }

    public decimal? NewPurchasePrice { get; set; }

    public decimal? NewSellingPrice { get; set; }

    public int? ChangedBy { get; set; }

    public DateTime? ChangedAt { get; set; }

    public virtual UserAccount? ChangedByNavigation { get; set; }

    public virtual Product? Product { get; set; }
}
