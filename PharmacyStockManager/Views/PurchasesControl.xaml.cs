using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using PharmacyStockManager.ViewModel;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class PurchasesControl : UserControl
    {
        public PurchasesControl()
        {
            InitializeComponent();
            PurchasesViewModel purchases = new PurchasesViewModel();
            this.DataContext = purchases;
        }
       
    }
}
