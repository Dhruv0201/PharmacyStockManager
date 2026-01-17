using PharmacyStockManager.Models;
using PharmacyStockManager.ViewModel;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace PharmacyStockManager.Views.PopupWindows
{
    /// <summary>
    /// Interaction logic for SaleDetailDialog.xaml
    /// </summary>
    public partial class SaleDetailDialog : ChildWindow
    {
        public AddEditSaleDetailViewModel ViewModel { get; set; }

        public SaleDetailDialog()
        {
            InitializeComponent();
            ViewModel = new AddEditSaleDetailViewModel();
            this.DataContext = ViewModel;
            ViewModel.CloseWindow += () =>
            {
                this.DialogResult = true;
                this.Close();
            };
        }

        public SaleDetailDialog(SaleDetail saleDetail)
        {
            InitializeComponent();
            ViewModel = new AddEditSaleDetailViewModel(saleDetail);
            this.DataContext = ViewModel;
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
