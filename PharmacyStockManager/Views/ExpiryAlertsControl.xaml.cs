using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class ExpiryAlertsControl : UserControl
    {
        private readonly AppDbContext _context;

        public ExpiryAlertsControl() => InitializeComponent();

        public ExpiryAlertsControl(AppDbContext context) : this()
        {
            _context = context;
            LoadAlerts();
        }

        private void LoadAlerts()
        {
            if (_context != null)
                dgExpiryAlerts.ItemsSource = _context.ExpiryAlerts
                    .Include(e => e.Product)
                    .ToList();
        }
    }
}
