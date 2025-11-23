using PharmacyStockManager.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    internal class AddEditPurchaseReturnDetailViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly AppDbContext _context = new AppDbContext();
        public event Action CloseWindow;

        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        private PurchaseReturnDetail _detail;
        public PurchaseReturnDetail Detail
        {
            get => _detail;
            set
            {
                _detail = value;
                OnPropertyChanged(nameof(Detail));
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

        private int _quantityReturned;
        public int QuantityReturned
        {
            get => _quantityReturned;
            set
            {
                _quantityReturned = value;
                OnPropertyChanged(nameof(QuantityReturned));
                CalculateTotal();
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
                CalculateTotal();
            }
        }

        private decimal? _totalPrice;
        public decimal? TotalPrice
        {
            get => _totalPrice;
            set
            {
                _totalPrice = value;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private bool isValidationOn;

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
                            return "Batch Number is required.";
                        break;

                    case nameof(QuantityReturned):
                        if (QuantityReturned <= 0)
                            return "Quantity must be greater than zero.";
                        break;

                    case nameof(UnitPrice):
                        if (UnitPrice <= 0)
                            return "Unit Price must be greater than zero.";
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
                    string err = this[p.Name];
                    if (!string.IsNullOrEmpty(err))
                        return true;
                }
                return false;
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditPurchaseReturnDetailViewModel()
        {
            BindProducts();
            SaveCommand = new RelayCommand(SaveDetail, obj => true);
            CancelCommand = new RelayCommand(obj => CloseWindow?.Invoke(), obj => true);
        }

        public AddEditPurchaseReturnDetailViewModel(int detailId) : this()
        {
            Detail = _context.PurchaseReturnDetails.Find(detailId);

            if (Detail != null)
            {
                SelectedProduct = Products.FirstOrDefault(p => p.ProductId == Detail.ProductId);
                BatchNumber = Detail.BatchNumber;
                QuantityReturned = Detail.QuantityReturned;
                UnitPrice = Detail.UnitPrice;
                TotalPrice = Detail.TotalPrice;
            }
        }

        private void BindProducts()
        {
            Products = new ObservableCollection<Product>(
                _context.Products
                .OrderBy(p => p.ProductName)
                .ToList());
        }

        private void SaveDetail(object obj)
        {
            isValidationOn = true;
            if (HasErrors)
                return;

            if (Detail != null)
            {
                Detail.ProductId = SelectedProduct.ProductId;
                Detail.BatchNumber = BatchNumber;
                Detail.QuantityReturned = QuantityReturned;
                Detail.UnitPrice = UnitPrice;
                Detail.TotalPrice = TotalPrice;
                Detail.ModifiedAt = DateTime.Now;
                Detail.ModifiedBy = App.LoggedInUser.UserId;
            }
            else
            {
                Detail = new PurchaseReturnDetail();
                Detail.ProductId = SelectedProduct.ProductId;
                Detail.BatchNumber = BatchNumber;
                Detail.QuantityReturned = QuantityReturned;
                Detail.UnitPrice = UnitPrice;
                Detail.TotalPrice = TotalPrice;

                _context.PurchaseReturnDetails.Add(Detail);
            }

            _context.SaveChanges();
            CloseWindow?.Invoke();
        }

        private void CalculateTotal()
        {
            if (QuantityReturned > 0 && UnitPrice > 0)
                TotalPrice = QuantityReturned * UnitPrice;
        }
    }
}
