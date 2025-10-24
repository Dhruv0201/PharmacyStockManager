using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class SaleReturnDetail
{
    public int SaleReturnDetailId { get; set; }

    public int? SaleReturnId { get; set; }

    public int? ProductId { get; set; }

    public string? BatchNumber { get; set; }

    public int QuantityReturned { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual UserAccount? ModifiedByNavigation { get; set; }

    public virtual Product? Product { get; set; }

    public virtual SaleReturnHeader? SaleReturn { get; set; }
}
