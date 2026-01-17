using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class ReturnProduct
{
    public int Id { get; set; }

    public int ReturnDetailId { get; set; }

    public int SaleDetailId { get; set; }

    public int ProductId { get; set; }

    public string? BatchNumber { get; set; }

    public int SoldQty { get; set; }

    public decimal SoldAmount { get; set; }

    public int ReturnQty { get; set; }

    public decimal ReturnAmount { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ReturnDetail ReturnDetail { get; set; } = null!;
}
