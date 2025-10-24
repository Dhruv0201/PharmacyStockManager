using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class PurchaseReturnsControl : UserControl
    {
        private readonly AppDbContext _context;

        public PurchaseReturnsControl() => InitializeComponent();

        public PurchaseReturnsControl(AppDbContext context) : this()
        {
            _context = context;
            LoadPurchaseReturns();
        }

        private void LoadPurchaseReturns()
        {
            if (_context != null)
                dgPurchaseReturns.ItemsSource = _context.PurchaseReturnHeaders
                    .Include(r => r.PurchaseReturnDetails)
                    .ToList();
        }
    }
}
