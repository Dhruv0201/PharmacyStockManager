using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class ReturnProduct
{
    public int Id { get; set; }

    public int ReturnDetailID { get; set; }

    public int SaleDetailID { get; set; }

    public int ProductID { get; set; }

    public string? BatchNumber { get; set; }

    public int SoldQty { get; set; }

    public decimal SoldAmount { get; set; }

    public int ReturnQty { get; set; }

    public decimal ReturnAmount { get; set; }

    public int? PurchaseDetailID { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual PurchaseDetail? PurchaseDetail { get; set; }

    public virtual ReturnDetail ReturnDetail { get; set; } = null!;
}
