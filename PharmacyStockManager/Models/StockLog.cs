using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class StockLog
{
    public int StockLogId { get; set; }

    public int? ProductId { get; set; }

    public string? BatchNumber { get; set; }

    public int Quantity { get; set; }

    public string ActionType { get; set; } = null!;

    public DateTime? ActionDate { get; set; }

    public int? PerformedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? Reason { get; set; }

    public virtual UserAccount? ModifiedByNavigation { get; set; }

    public virtual UserAccount? PerformedByNavigation { get; set; }

    public virtual Product? Product { get; set; }
}
