using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyStockManager.Models;

public partial class UserAccount
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string HashedPassword { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public string Role { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<Category> CategoryCreatedByNavigations { get; set; } = new List<Category>();

    public virtual ICollection<Category> CategoryModifiedByNavigations { get; set; } = new List<Category>();

    public virtual UserAccount? CreatedByNavigation { get; set; }

    public virtual ICollection<UserAccount> InverseCreatedByNavigation { get; set; } = new List<UserAccount>();

    public virtual ICollection<UserAccount> InverseModifiedByNavigation { get; set; } = new List<UserAccount>();

    public virtual UserAccount? ModifiedByNavigation { get; set; }

    public virtual ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

    public virtual ICollection<Purchase> PurchaseModifiedByNavigations { get; set; } = new List<Purchase>();

    public virtual ICollection<Purchase> PurchasePurchasedByNavigations { get; set; } = new List<Purchase>();

    public virtual ICollection<PurchaseReturnDetail> PurchaseReturnDetails { get; set; } = new List<PurchaseReturnDetail>();

    public virtual ICollection<PurchaseReturnHeader> PurchaseReturnHeaderModifiedByNavigations { get; set; } = new List<PurchaseReturnHeader>();

    public virtual ICollection<PurchaseReturnHeader> PurchaseReturnHeaderReturnedByNavigations { get; set; } = new List<PurchaseReturnHeader>();

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();

    public virtual ICollection<Sale> SaleModifiedByNavigations { get; set; } = new List<Sale>();

    public virtual ICollection<SalePayment> SalePaymentCollectedByNavigations { get; set; } = new List<SalePayment>();

    public virtual ICollection<SalePayment> SalePaymentModifiedByNavigations { get; set; } = new List<SalePayment>();

    public virtual ICollection<SaleReturnDetail> SaleReturnDetails { get; set; } = new List<SaleReturnDetail>();

    public virtual ICollection<SaleReturnHeader> SaleReturnHeaderModifiedByNavigations { get; set; } = new List<SaleReturnHeader>();

    public virtual ICollection<SaleReturnHeader> SaleReturnHeaderReturnedByNavigations { get; set; } = new List<SaleReturnHeader>();

    public virtual ICollection<Sale> SaleSoldByNavigations { get; set; } = new List<Sale>();

    public virtual ICollection<StockLog> StockLogModifiedByNavigations { get; set; } = new List<StockLog>();

    public virtual ICollection<StockLog> StockLogPerformedByNavigations { get; set; } = new List<StockLog>();

    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}
