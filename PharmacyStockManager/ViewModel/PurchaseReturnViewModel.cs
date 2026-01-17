using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    public class PurchaseReturnViewModel : ViewModelBase
    {
        AppDbContext appDbContext = new AppDbContext();

        private DateTime? _returnDate = DateTime.Now;
        private Product SelectedProduct;
        private PurchaseDetail selectedPurchaseDetail;
        private ReturnDetail returnDetail;
        private Purchase purchase;
        private ReturnProduct SelectedReturnProduct;

        public DateTime? ReturnDate
        {
            get => _returnDate;
            set { _returnDate = value; OnPropertyChanged(nameof(ReturnDate)); }
        }

        private string _supplierName;
        public string SupplierName
        {
            get => _supplierName;
            set { _supplierName = value; OnPropertyChanged(nameof(SupplierName)); }
        }

        private string _reason;
        public string Reason
        {
            get => _reason;
            set { _reason = value; OnPropertyChanged(nameof(Reason)); }
        }

        private int purchasedQty;
        public int PurchasedQty
        {
            get => purchasedQty;
            set { purchasedQty = value; OnPropertyChanged(nameof(PurchasedQty)); }
        }

        private string _selectedProductName;
        public string SelectedProductName
        {
            get => _selectedProductName;
            set { _selectedProductName = value; OnPropertyChanged(nameof(SelectedProductName)); }
        }

        private string _batchNumber;
        public string BatchNumber
        {
            get => _batchNumber;
            set { _batchNumber = value; OnPropertyChanged(nameof(BatchNumber)); }
        }

        private int _returnQty;
        public int ReturnQty
        {
            get => _returnQty;
            set { _returnQty = value; OnPropertyChanged(nameof(ReturnQty)); CalculateTotal(); }
        }

        private decimal _unitCost;
        public decimal UnitCost
        {
            get => _unitCost;
            set { _unitCost = value; OnPropertyChanged(nameof(UnitCost)); CalculateTotal(); }
        }

        private decimal _returnAmountForProduct;
        public decimal ReturnAmountForProduct
        {
            get => _returnAmountForProduct;
            set { _returnAmountForProduct = value; OnPropertyChanged(nameof(ReturnAmountForProduct)); }
        }

        private decimal _totalReturnAmount;
        public decimal TotalReturnAmount
        {
            get => _totalReturnAmount;
            set { _totalReturnAmount = value; OnPropertyChanged(nameof(TotalReturnAmount)); }
        }

        private decimal _purchaseAmount;
        public decimal PurchaseAmount
        {
            get => _purchaseAmount;
            set { _purchaseAmount = value; OnPropertyChanged(nameof(PurchaseAmount)); }
        }

        public ObservableCollection<PurchaseDetail> PurchasedItems { get; set; } = new();

        private ObservableCollection<ReturnProduct> _returnItems = new();
        public ObservableCollection<ReturnProduct> ReturnItems
        {
            get => _returnItems;
            set { _returnItems = value; OnPropertyChanged(nameof(ReturnItems)); }
        }

        public ICommand EditReturnItemCommand { get; }
        public ICommand DeleteReturnItemCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand PurchasedItemDoubleClickCommand { get; }
        public ICommand ReturnCommand { get; }

        public Action CloseWindow;

        public PurchaseReturnViewModel(int PurchaseID) : this()
        {
            purchase = appDbContext.Purchases
                .Include(p => p.PurchaseDetails)
                    .ThenInclude(pd => pd.Product)
                .Include(p => p.Supplier)
                .FirstOrDefault(x => x.PurchaseID == PurchaseID);

            SupplierName = purchase.Supplier?.SupplierName;
            PurchasedItems = new ObservableCollection<PurchaseDetail>(purchase.PurchaseDetails);
        }

        public PurchaseReturnViewModel()
        {
            EditReturnItemCommand = new RelayCommand(EditReturnItem);
            DeleteReturnItemCommand = new RelayCommand(DeleteReturnItem);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            PurchasedItemDoubleClickCommand = new RelayCommand(SetProduct, obj => obj != null);
            ReturnCommand = new RelayCommand(ReturnItemCommand, IsReturnPossible);
        }

        private bool IsReturnPossible(object parameter)
        {
            return (SelectedProduct != null || SelectedReturnProduct != null)
                   && ReturnQty > 0
                   && ReturnQty <= purchasedQty;
        }

        private void ReturnItemCommand(object obj)
        {
            if (SelectedReturnProduct != null)
            {
                SelectedReturnProduct.ReturnQty = ReturnQty;
                SelectedReturnProduct.ReturnAmount = ReturnAmountForProduct;

                ObservableCollection<ReturnProduct> temp = new(ReturnItems);
                ReturnItems.Clear();
                ReturnItems = temp;

                SelectedProduct = null;
            }
            else
            {
                ReturnProduct returnProduct = new()
                {
                    Product = SelectedProduct,
                    ProductID = SelectedProduct.ProductID,
                    ReturnAmount = ReturnAmountForProduct,
                    ReturnQty = ReturnQty,
                    PurchaseDetailID = selectedPurchaseDetail.PurchaseDetailID,
                    BatchNumber = BatchNumber,
                    SoldAmount = selectedPurchaseDetail.UnitPrice,
                    SoldQty= selectedPurchaseDetail.Quantity
                };

                if (returnDetail == null)
                    returnDetail = new ReturnDetail();

                returnDetail.ReturnProducts.Add(returnProduct);
                ReturnItems.Add(returnProduct);

                SelectedProduct = null;
            }

            SelectedProductName = string.Empty;
            BatchNumber = string.Empty;
            UnitCost = 0;
            ReturnQty = 0;
            PurchasedQty = 0;
            PurchaseAmount = 0;

            TotalReturnAmount = ReturnItems.Sum(rp => rp.ReturnAmount);
        }

        private void SetProduct(object obj)
        {
            if (obj is PurchaseDetail item)
            {
                selectedPurchaseDetail = item;
                SelectedProduct = item.Product;

                SelectedProductName = SelectedProduct.ProductName;
                BatchNumber = item.BatchNumber;
                UnitCost = item.UnitPrice;
                PurchaseAmount = item.TotalPrice ?? 0;
                PurchasedQty = item.Quantity;
            }
        }

        private void EditReturnItem(object obj)
        {
            if (obj is ReturnProduct item)
            {
                SelectedReturnProduct = item;
                SelectedProductName = item.Product.ProductName;
                BatchNumber = item.BatchNumber;
                PurchasedQty = item.SoldQty;
                ReturnQty = item.ReturnQty;
                UnitCost = item.SoldAmount;
                ReturnAmountForProduct = item.ReturnAmount;
                PurchaseAmount = item.SoldAmount;
            }
        }

        private void DeleteReturnItem(object obj)
        {
            if (obj is ReturnProduct item)
                ReturnItems.Remove(item);

            TotalReturnAmount = ReturnItems.Sum(rp => rp.ReturnAmount);
        }

        private void Save(object obj)
        {
            foreach (var returnProduct in ReturnItems)
            {
                PurchaseDetail purchaseDetail = purchase.PurchaseDetails.FirstOrDefault(x => x.ProductID == returnProduct.ProductID);
                if (purchaseDetail != null)
                {
                    purchaseDetail.Quantity = purchaseDetail.Quantity- returnProduct.ReturnQty;
                    purchaseDetail.TotalPrice = purchaseDetail.TotalPrice - returnProduct.ReturnAmount;
                    appDbContext.PurchaseDetails.Update(purchaseDetail);
                }
            }
            PurchaseReturnHeader purchaseReturnHeader = new PurchaseReturnHeader
            {
                PurchaseID = purchase.PurchaseID,
                SupplierID = purchase.SupplierID,
                ReturnDate = ReturnDate,
                TotalAmount = TotalReturnAmount,
                ReturnedBy = App.LoggedInUser.UserID,
                ModifiedBy = App.LoggedInUser.UserID,
                ModifiedAt = DateTime.Now,
                Reason = Reason
            };
            appDbContext.PurchaseReturnHeaders.Add(purchaseReturnHeader);
            appDbContext.SaveChanges();
           foreach (var returnProduct in ReturnItems)
            {
                PurchaseReturnDetail purchaseReturnDetail = new PurchaseReturnDetail
                {
                    PurchaseReturnID = purchaseReturnHeader.PurchaseReturnID,
                    ProductID = returnProduct.ProductID,
                    BatchNumber = returnProduct.BatchNumber,
                   QuantityReturned = returnProduct.ReturnQty,
                    UnitPrice = returnProduct.SoldAmount,
                    TotalPrice = returnProduct.ReturnAmount,
                    ModifiedBy = App.LoggedInUser.UserID,
                    ModifiedAt = DateTime.Now
                };
                appDbContext.PurchaseReturnDetails.Add(purchaseReturnDetail);
            }
            appDbContext.SaveChanges();
            CloseWindow?.Invoke();
        }

        private void Cancel(object obj)
        {
            CloseWindow?.Invoke();
        }

        private void CalculateTotal()
        {
            ReturnAmountForProduct = ReturnQty * UnitCost;
        }
    }
}
