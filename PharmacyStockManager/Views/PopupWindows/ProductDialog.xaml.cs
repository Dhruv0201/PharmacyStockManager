using PharmacyStockManager.Models;
using PharmacyStockManager.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace PharmacyStockManager.Views.PopupWindows
{
    public partial class ProductDialog : Window
    {
        public ObservableCollection<Category> Categories { get; }
        public ObservableCollection<Supplier> Suppliers { get; }

        public bool IsEditMode { get; }

        // Properties bound to UI
        public string ProductName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public string? BatchNumber { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }

        public ProductDialog()
        {
            InitializeComponent();
            AddEditProductViewModel viewModel = new AddEditProductViewModel();
            this.DataContext = viewModel;
        }

        public ProductDialog(int productId)
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void txtPurchasePrice_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
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
