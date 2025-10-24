using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class SalePayment
{
    public int PaymentId { get; set; }

    public int? SaleId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentMode { get; set; }

    public decimal AmountPaid { get; set; }

    public bool? IsCredit { get; set; }

    public decimal? DueAmount { get; set; }

    public int? CollectedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual UserAccount? CollectedByNavigation { get; set; }

    public virtual UserAccount? ModifiedByNavigation { get; set; }

    public virtual Sale? Sale { get; set; }
}
