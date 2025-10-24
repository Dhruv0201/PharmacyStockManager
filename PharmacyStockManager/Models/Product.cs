using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int? CategoryId { get; set; }

    public int? SupplierId { get; set; }

    public string? BatchNumber { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal SellingPrice { get; set; }

    public int QuantityInStock { get; set; }

    public int ReorderLevel { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<ExpiryAlert> ExpiryAlerts { get; set; } = new List<ExpiryAlert>();

    public virtual UserAccount? ModifiedByNavigation { get; set; }

    public virtual ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();

    public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

    public virtual ICollection<PurchaseReturnDetail> PurchaseReturnDetails { get; set; } = new List<PurchaseReturnDetail>();

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();

    public virtual ICollection<SaleReturnDetail> SaleReturnDetails { get; set; } = new List<SaleReturnDetail>();

    public virtual ICollection<StockLog> StockLogs { get; set; } = new List<StockLog>();

    public virtual Supplier? Supplier { get; set; }
}
