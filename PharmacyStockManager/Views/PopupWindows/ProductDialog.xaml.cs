using PharmacyStockManager.Models;
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

        public ProductDialog(ObservableCollection<Category> categories, ObservableCollection<Supplier> suppliers, Product? existingProduct = null)
        {
            InitializeComponent();

            Categories = categories;
            Suppliers = suppliers;

            DataContext = this;

            if (existingProduct != null)
            {
                IsEditMode = true;
                ProductName = existingProduct.ProductName;
                CategoryId = existingProduct.CategoryId ?? 0;
                SupplierId = existingProduct.SupplierId ?? 0;
                BatchNumber = existingProduct.BatchNumber;
                ExpiryDate = existingProduct.ExpiryDate;
                PurchasePrice = existingProduct.PurchasePrice;
                SellingPrice = existingProduct.SellingPrice;
                QuantityInStock = existingProduct.QuantityInStock;
                ReorderLevel = existingProduct.ReorderLevel;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductName))
            {
                MessageBox.Show("Product name is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CategoryId == 0)
            {
                MessageBox.Show("Please select a category.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SupplierId == 0)
            {
                MessageBox.Show("Please select a supplier.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
