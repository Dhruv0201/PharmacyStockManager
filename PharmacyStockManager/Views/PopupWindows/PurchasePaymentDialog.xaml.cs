using PharmacyStockManager.Models;
using PharmacyStockManager.ViewModel;
using Xceed.Wpf.Toolkit;

namespace PharmacyStockManager.Views.PopupWindows
{
    /// <summary>
    /// Interaction logic for PurchasePaymentDialog.xaml
    /// </summary>
    public partial class PurchasePaymentDialog : ChildWindow
    {
        public AddEditPurchasePaymentViewModel ViewModel { get; set; }
        public PurchasePaymentDialog(decimal totalAmount, decimal paidAmount)
        {
            InitializeComponent();
            ViewModel = new AddEditPurchasePaymentViewModel(totalAmount, paidAmount);
            DataContext = ViewModel;
            ViewModel.CloseWindow += () =>
            {
                this.DialogResult = true;
                this.Close();
            };
        }

        public PurchasePaymentDialog(PurchasePayment payment, decimal totalAmount, decimal paidAmount)
        {
            InitializeComponent();
            ViewModel = new AddEditPurchasePaymentViewModel(payment, totalAmount, paidAmount);
            DataContext = ViewModel;
            ViewModel.CloseWindow += () =>
            {
                this.DialogResult = true;
                this.Close();
            };
        }
    }
}
