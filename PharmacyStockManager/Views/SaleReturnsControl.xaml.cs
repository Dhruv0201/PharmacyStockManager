using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class SaleReturnsControl : UserControl
    {
        private readonly AppDbContext _context;

        public SaleReturnsControl() => InitializeComponent();

        public SaleReturnsControl(AppDbContext context) : this()
        {
            _context = context;
            LoadSaleReturns();
        }

        private void LoadSaleReturns()
        {
            if (_context != null)
                dgSaleReturns.ItemsSource = _context.SaleReturnHeaders
                    .Include(r => r.SaleReturnDetails)
                    .ToList();
        }
    }
}
