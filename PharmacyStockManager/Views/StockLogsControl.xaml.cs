using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class StockLogsControl : UserControl
    {
        private readonly AppDbContext _context;

        public StockLogsControl() => InitializeComponent();

        public StockLogsControl(AppDbContext context) : this()
        {
            _context = context;
            LoadStockLogs();
        }

        private void LoadStockLogs()
        {
            if (_context != null)
                dgStockLogs.ItemsSource = _context.StockLogs
                    .Include(sl => sl.Product)
                    .Include(sl => sl.PerformedByNavigation)
                    .ToList();
        }
    }
}
