using System;
using PharmacyStockManager.ViewModel;

namespace PharmacyStockManager.ViewModel
{
    public class ProductItemViewModel : ViewModelBase
    {
        public int SerialNumber { get; set; }
        public int ProductId { get; set; }

        private string _productName = string.Empty;
        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged(nameof(ProductName));
            }
        }

        public string CategoryName { get; set; } = "Unknown";
        public string SupplierName { get; set; } = "Unknown";

        public string? BatchNumber { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }

        public string ModifiedByName { get; set; } = "Unknown";
        public DateTime? ModifiedAt { get; set; }
    }
}
