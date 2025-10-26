using PharmacyStockManager.Models;
using PharmacyStockManager.ViewModel;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class ProductsControl : UserControl
    {
        public ProductsControl()
        {
            InitializeComponent();
            this.DataContext = new ProductsViewModel();
        }
    }
}
