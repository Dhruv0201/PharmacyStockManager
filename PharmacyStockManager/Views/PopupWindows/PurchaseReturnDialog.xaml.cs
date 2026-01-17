using PharmacyStockManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace PharmacyStockManager.Views.PopupWindows
{
    /// <summary>
    /// Interaction logic for PurchaseReturnDialog.xaml
    /// </summary>
    public partial class PurchaseReturnDialog : ChildWindow
    {
        public PurchaseReturnDialog()
        {
            InitializeComponent();
        }

        public PurchaseReturnDialog(int PurchaseID)
        {
            InitializeComponent();
            PurchaseReturnViewModel viewModel = new PurchaseReturnViewModel(PurchaseID);
            this.DataContext = viewModel;

        }
    }
}
