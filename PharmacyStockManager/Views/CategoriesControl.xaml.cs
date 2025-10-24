using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class CategoriesControl : UserControl
    {
        private readonly AppDbContext _context;

        public CategoriesControl()
        {
            InitializeComponent();
        }

        public CategoriesControl(AppDbContext context) : this()
        {
            _context = context;
            LoadProducts();
        }

        private void LoadProducts()
        {
            if (_context != null)
            {
                dgCategories.ItemsSource = _context.Categories.ToList();
            }
        }
    }
}