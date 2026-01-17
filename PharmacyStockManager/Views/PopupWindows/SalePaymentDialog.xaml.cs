using PharmacyStockManager.Models;
using PharmacyStockManager.ViewModel;
using Xceed.Wpf.Toolkit;

namespace PharmacyStockManager.Views.PopupWindows
{
    /// <summary>
    /// Interaction logic for PaymentDialog.xaml
    /// </summary>
    public partial class SalePaymentDialog : ChildWindow
    {
        public AddEditSalePaymentViewModel ViewModel { get; set; }

        public SalePaymentDialog(decimal totalAmount, decimal paidAmount)
        {
            InitializeComponent();

            ViewModel = new AddEditSalePaymentViewModel(totalAmount, paidAmount);
            DataContext = ViewModel;

            ViewModel.CloseWindow += () =>
            {
                this.DialogResult = true;
                this.Close();
            };
        }

        public SalePaymentDialog(SalePayment payment, decimal totalAmount, decimal paidAmmoun)
        {
            InitializeComponent();

            ViewModel = new AddEditSalePaymentViewModel(payment, totalAmount, paidAmmoun);
            DataContext = ViewModel;

            ViewModel.CloseWindow += () =>
            {
                this.DialogResult = true;
                this.Close();
            };
        }
    }
}