using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using PharmacyStockManager.ViewModel;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class SuppliersControl : UserControl
    {
        SuppliersViewModel viewModel;
        public SuppliersControl()
        {
            InitializeComponent();
            viewModel = new SuppliersViewModel();
            this.DataContext = viewModel;
        }
    }
}
