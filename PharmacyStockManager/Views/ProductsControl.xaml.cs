using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class ProductsControl : UserControl
    {
        private readonly AppDbContext _context;

        // Parameterless constructor for XAML designer
        public ProductsControl()
        {
            InitializeComponent();
        }

        // Constructor with DbContext
        public ProductsControl(AppDbContext context) : this()
        {
            _context = context;
            LoadProducts();
        }

        private void LoadProducts()
        {
            if (_context != null)
            {
                dgProducts.ItemsSource = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .ToList();
            }
        }
    }
}