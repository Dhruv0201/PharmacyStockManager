using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual ICollection<SaleReturnHeader> SaleReturnHeaders { get; set; } = new List<SaleReturnHeader>();

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
