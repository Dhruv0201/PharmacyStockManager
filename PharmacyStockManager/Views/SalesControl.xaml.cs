using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using System.Linq;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class SalesControl : UserControl
    {
        private readonly AppDbContext _context;

        public SalesControl()
        {
            InitializeComponent();
        }

        public SalesControl(AppDbContext context) : this()
        {
            _context = context;
            LoadSales();
        }

        private void LoadSales()
        {
            if (_context != null)
            {
                // Include SaleDetails -> Product
                var sales = _context.Sales
                    .Include(s => s.SoldByNavigation)
                    .Include(s => s.Customer)
                    .Include(s => s.SaleDetails)
                        .ThenInclude(sd => sd.Product)
                    .ToList();

                // Flatten data for DataGrid
                var salesFlattened = sales.SelectMany(s => s.SaleDetails.Select(sd => new
                {
                    SaleID = s.SaleId,
                    ProductName = sd.Product.ProductName,
                    Quantity = sd.QuantitySold,
                    SaleDate = s.SaleDate,
                    SoldBy = s.SoldByNavigation
                })).ToList();

                dgSales.ItemsSource = salesFlattened;
            }
        }
    }
}
