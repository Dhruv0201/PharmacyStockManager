using PharmacyStockManager.Models;
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
    /// Interaction logic for PaymentDialog.xaml
    /// </summary>
    public partial class PaymentDialog : ChildWindow
    {
        public AddEditPaymentViewModel ViewModel { get; set; }

        public PaymentDialog(decimal totalAmount,decimal paidAmount)
        {
            InitializeComponent();

            ViewModel = new AddEditPaymentViewModel(totalAmount,paidAmount);
            DataContext = ViewModel;

            ViewModel.CloseWindow += () =>
            {
                this.DialogResult = true;
                this.Close();
            };
        }

        public PaymentDialog(SalePayment payment, decimal totalAmount,decimal paidAmmoun)
        {
            InitializeComponent();

            ViewModel = new AddEditPaymentViewModel(payment, totalAmount,paidAmmoun);
            DataContext = ViewModel;

            ViewModel.CloseWindow += () =>
            {
                this.DialogResult = true;
                this.Close();
            };
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(ch => char.IsDigit(ch) || ch == '.');
        }

        private void NumericTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = e.DataObject.GetData(DataFormats.Text) as string;
                if (!text.All(char.IsDigit))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}