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
    /// Interaction logic for PurchaseReturnDetailDialog.xaml
    /// </summary>
    public partial class PurchaseReturnDetailDialog : ChildWindow
    {
        public PurchaseReturnDetailDialog()
        {
            InitializeComponent();
            AddEditPurchaseReturnDetailViewModel addEditPurchaseReturnDetailViewModel = new AddEditPurchaseReturnDetailViewModel();
            this.DataContext = addEditPurchaseReturnDetailViewModel;
            addEditPurchaseReturnDetailViewModel.CloseWindow += AddEditPurchaseReturnDetailViewModel_CloseWindow;
        }

        private void AddEditPurchaseReturnDetailViewModel_CloseWindow()
        {
            this.Close();
        }

        public PurchaseReturnDetailDialog(int detailId)
        {
            InitializeComponent();
            AddEditPurchaseReturnDetailViewModel addEditPurchaseReturnDetailViewModel = new AddEditPurchaseReturnDetailViewModel(detailId);
            this.DataContext = addEditPurchaseReturnDetailViewModel;
            addEditPurchaseReturnDetailViewModel.CloseWindow += AddEditPurchaseReturnDetailViewModel_CloseWindow;
        }

        private void txtQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(ch => char.IsDigit(ch) || ch == '.');
        }
    }
}
