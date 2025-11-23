using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using PharmacyStockManager.ViewModel;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class PurchaseReturnsControl : UserControl
    {
        public PurchaseReturnsControl()
        {
            InitializeComponent();
            PurchaseReturnViewModel viewModel = new PurchaseReturnViewModel();
            this.DataContext = viewModel;
        }
        
    }
}
