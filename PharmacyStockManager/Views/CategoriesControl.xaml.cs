using PharmacyStockManager.Models;
using PharmacyStockManager.ViewModel;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class CategoriesControl : UserControl
    {

        public CategoriesControl()
        {
            InitializeComponent();

            // Assign DataContext to the ViewModel, passing context there
            this.DataContext = new CategoriesViewModel();
        }
    }
}
