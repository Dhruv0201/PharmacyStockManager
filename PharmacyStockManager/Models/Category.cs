using System;
using System.Collections.Generic;

namespace PharmacyStockManager.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual UserAccount? CreatedByNavigation { get; set; }

    public virtual UserAccount? ModifiedByNavigation { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
