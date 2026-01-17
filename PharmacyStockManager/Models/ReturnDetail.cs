using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class ReturnDetail
{
    public int Id { get; set; }

    public int SaleID { get; set; }

    public DateTime ReturnDate { get; set; }

    public int? CustomerID { get; set; }

    public decimal ReturnAmount { get; set; }

    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<ReturnProduct> ReturnProducts { get; set; } = new List<ReturnProduct>();

    public virtual Sale Sale { get; set; } = null!;
}
