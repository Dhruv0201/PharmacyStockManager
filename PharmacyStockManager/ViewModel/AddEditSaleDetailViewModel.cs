using PharmacyStockManager.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    public class AddEditSaleDetailViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly AppDbContext _context = new AppDbContext();
        public event Action CloseWindow;

        public ObservableCollection<Product> Products { get; set; }

        private SaleDetail saleDetail;
        public SaleDetail SaleDetail
        {
            get => saleDetail;
            set
            {
                saleDetail = value;
                OnPropertyChanged(nameof(SaleDetail));
            }
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }

        private string _batchNumber;
        public string BatchNumber
        {
            get => _batchNumber;
            set
            {
                _batchNumber = value;
                OnPropertyChanged(nameof(BatchNumber));
            }
        }

        private int _quantitySold;
        public int QuantitySold
        {
            get => _quantitySold;
            set
            {
                _quantitySold = value;
                OnPropertyChanged(nameof(QuantitySold));
                RecalculateTotal();
            }
        }

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                _unitPrice = value;
                OnPropertyChanged(nameof(UnitPrice));
                RecalculateTotal();
            }
        }

        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get => _totalPrice;
            set
            {
                _totalPrice = value;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private bool isValidationOn = false;

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if (!isValidationOn)
                    return null;

                switch (columnName)
                {
                    case nameof(SelectedProduct):
                        if (SelectedProduct == null)
                            return "Product is required.";
                        break;

                    case nameof(BatchNumber):
                        if (string.IsNullOrWhiteSpace(BatchNumber))
                            return "Batch number is required.";
                        break;

                    case nameof(QuantitySold):
                        if (QuantitySold <= 0)
                            return "Quantity must be greater than zero.";
                        break;

                    case nameof(UnitPrice):
                        if (UnitPrice <= 0)
                            return "Unit price must be greater than zero.";
                        break;
                }

                return string.Empty;
            }
        }

        public bool HasErrors
        {
            get
            {
                OnPropertyChanged(null);
                var props = GetType().GetProperties();

                foreach (var p in props)
                {
                    if (!string.IsNullOrEmpty(this[p.Name]))
                        return true;
                }

                return false;
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditSaleDetailViewModel()
        {
            Products = new ObservableCollection<Product>(_context.Products.OrderBy(p => p.ProductName).ToList());
            SaveCommand = new RelayCommand(SaveItem, obj => true);
            CancelCommand = new RelayCommand(obj => CloseWindow?.Invoke(), obj => true);
        }

        public AddEditSaleDetailViewModel(SaleDetail saleDetail) : this()
        {
            this.saleDetail = saleDetail;

            if (saleDetail != null)
            {
                SelectedProduct = Products.FirstOrDefault(p => p.ProductId == saleDetail.ProductId);
                BatchNumber = saleDetail.BatchNumber;
                QuantitySold = saleDetail.QuantitySold;
                UnitPrice = saleDetail.UnitPrice;
                TotalPrice = saleDetail.TotalPrice ?? (saleDetail.UnitPrice * saleDetail.QuantitySold);
            }
        }

        private void RecalculateTotal()
        {
            TotalPrice = UnitPrice * QuantitySold;
        }

        private void SaveItem(object obj)
        {
            isValidationOn = true;

            if (HasErrors)
                return;

            if (saleDetail != null)
            {
                saleDetail.ProductId = SelectedProduct.ProductId;
                saleDetail.BatchNumber = BatchNumber;
                saleDetail.QuantitySold = QuantitySold;
                saleDetail.UnitPrice = UnitPrice;
                saleDetail.TotalPrice = TotalPrice;
                saleDetail.ModifiedAt = DateTime.Now;
                saleDetail.ModifiedBy = App.LoggedInUser.UserId;
                saleDetail.Product = SelectedProduct;
            }
            else
            {
                saleDetail = new SaleDetail()
                {
                    ProductId = SelectedProduct.ProductId,
                    BatchNumber = BatchNumber,
                    QuantitySold = QuantitySold,
                    UnitPrice = UnitPrice,
                    TotalPrice = TotalPrice,
                    Product = SelectedProduct


                };
            }

            CloseWindow?.Invoke();
        }
    }
}
