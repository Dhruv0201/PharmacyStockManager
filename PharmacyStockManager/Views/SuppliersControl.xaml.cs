using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class SuppliersControl : UserControl
    {
        private readonly AppDbContext _context;

        public SuppliersControl() => InitializeComponent();

        public SuppliersControl(AppDbContext context) : this()
        {
            _context = context;
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            if (_context != null)
                dgSuppliers.ItemsSource = _context.Suppliers.ToList();
        }
    }
}
