using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class PurchasesControl : UserControl
    {
        private readonly AppDbContext _context;

        public PurchasesControl() => InitializeComponent();

        public PurchasesControl(AppDbContext context) : this()
        {
            _context = context;
            LoadPurchases();
        }

        private void LoadPurchases()
        {
            if (_context != null)
                dgPurchases.ItemsSource = _context.Purchases
                    .Include(p => p.Supplier)
                    .Include(p => p.PurchasedBy)
                    .ToList();
        }
    }
}
