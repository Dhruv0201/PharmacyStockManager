using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class PurchasePayment
{
    public int PaymentID { get; set; }

    public int? PurchaseID { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentMode { get; set; }

    public decimal AmountPaid { get; set; }

    public bool? IsCredit { get; set; }

    public decimal? DueAmount { get; set; }

    public int? PaidBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual UserAccount? ModifiedByNavigation { get; set; }

    public virtual UserAccount? PaidByNavigation { get; set; }

    public virtual Purchase? Purchase { get; set; }
}
