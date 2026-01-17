using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class SaleReturnHeader
{
    public int SaleReturnID { get; set; }

    public int? SaleID { get; set; }

    public int? CustomerID { get; set; }

    public DateTime? ReturnDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? ReturnedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? Reason { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual UserAccount? ModifiedByNavigation { get; set; }

    public virtual UserAccount? ReturnedByNavigation { get; set; }

    public virtual Sale? Sale { get; set; }

    public virtual ICollection<SaleReturnDetail> SaleReturnDetails { get; set; } = new List<SaleReturnDetail>();
}
