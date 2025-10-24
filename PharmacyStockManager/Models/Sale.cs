using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class Sale
{
    public int SaleId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime? SaleDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? SoldBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual UserAccount? ModifiedByNavigation { get; set; }

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();

    public virtual ICollection<SalePayment> SalePayments { get; set; } = new List<SalePayment>();

    public virtual ICollection<SaleReturnHeader> SaleReturnHeaders { get; set; } = new List<SaleReturnHeader>();

    public virtual UserAccount? SoldByNavigation { get; set; }
}
