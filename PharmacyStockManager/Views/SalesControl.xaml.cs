using PharmacyStockManager.ViewModel;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class SalesControl : UserControl
    {
        public SalesControl()
        {
            InitializeComponent();
            SalesViewModel viewModel = new SalesViewModel();
            this.DataContext = viewModel;
        }
    }
}
