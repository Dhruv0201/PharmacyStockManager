using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class ExpiryAlert
{
    public int ExpiryAlertID { get; set; }

    public int? ProductID { get; set; }

    public string? BatchNumber { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public int? Quantity { get; set; }

    public DateTime? AlertDate { get; set; }

    public bool? Notified { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual Product? Product { get; set; }
}
