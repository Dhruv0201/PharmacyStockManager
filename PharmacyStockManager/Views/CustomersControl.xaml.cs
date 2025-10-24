using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class CustomersControl : UserControl
    {
        private readonly AppDbContext _context;

        public CustomersControl() => InitializeComponent();

        public CustomersControl(AppDbContext context) : this()
        {
            _context = context;
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            if (_context != null)
                dgCustomers.ItemsSource = _context.Customers.ToList();
        }
    }
}
