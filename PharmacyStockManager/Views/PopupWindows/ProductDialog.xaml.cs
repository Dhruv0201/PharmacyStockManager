using PharmacyStockManager.Models;
using PharmacyStockManager.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace PharmacyStockManager.Views.PopupWindows
{
    public partial class ProductDialog : ChildWindow
    {
        public ProductDialog()
        {
            InitializeComponent();
            AddEditProductViewModel viewModel = new AddEditProductViewModel();
            this.DataContext = viewModel;
            viewModel.CloseWindow += () =>
            {
                this.DialogResult = true;
                this.Close();
            };
        }

        public ProductDialog(int productId)
        {
            InitializeComponent();
            AddEditProductViewModel viewModel = new AddEditProductViewModel(productId);
            this.DataContext = viewModel;
            viewModel.CloseWindow += () =>
            {
                this.DialogResult = true;
                this.Close();
            };
        }

        private void txtPurchasePrice_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(ch=> char.IsDigit(ch) || ch == '.');
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
