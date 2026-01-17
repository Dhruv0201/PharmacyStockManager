using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    public class SaleReturnViewModel : ViewModelBase
    {
        AppDbContext appDbContext = new AppDbContext();
        private DateTime? _returnDate = DateTime.Now;
        private Product SelectedProduct;
        private SaleDetail selectedSaleDetail;
        private ReturnDetail returnDetail;
        private Sale sale;
        private ReturnProduct SelectedReturnProduct;

        public DateTime? ReturnDate
        {
            get => _returnDate;
            set { _returnDate = value; OnPropertyChanged(nameof(ReturnDate)); }
        }

        private string _customerName;
        public string CustomerName
        {
            get => _customerName;
            set { _customerName = value; OnPropertyChanged(nameof(CustomerName)); }
        }

        private string _reason;
        public string Reason
        {
            get => _reason;
            set
            {
                _reason = value;
                OnPropertyChanged(nameof(Reason));
            }
        }

        private int soldQty;
        public int SoldQty
        {
            get { return soldQty; }
            set { soldQty = value; OnPropertyChanged((nameof(SoldQty))); }

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

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get => _unitPrice;
            set { _unitPrice = value; OnPropertyChanged(nameof(UnitPrice)); CalculateTotal(); }
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

        private decimal _saleAmount;
        public decimal SaleAmount
        {
            get => _saleAmount;
            set { _saleAmount = value; OnPropertyChanged(nameof(SaleAmount)); }
        }

        public ObservableCollection<SaleDetail> SoldItems
        {
            get;
            set;
        } = new();
        private ObservableCollection<ReturnProduct> _returnItems = new ObservableCollection<ReturnProduct>();
        public ObservableCollection<ReturnProduct> ReturnItems
        {
            get => _returnItems;
            set
            {
                _returnItems = value;
                OnPropertyChanged(nameof(ReturnItems));
            }
        }


        public ICommand EditReturnItemCommand { get; }
        public ICommand DeleteReturnItemCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SoldItemDoubleClickCommand { get; }
        public ICommand ReturnCommand { get; }

        public Action CloseWindow;


        public SaleReturnViewModel(int SaleId) : this()
        {
            sale = appDbContext.Sales.Include(s => s.SaleDetails).ThenInclude(sd => sd.Product).Include(s => s.Customer).FirstOrDefault(x => x.SaleId == SaleId);
            CustomerName = sale.Customer?.Name;
            SoldItems = new ObservableCollection<SaleDetail>(sale.SaleDetails);

        }
        public SaleReturnViewModel()
        {
            EditReturnItemCommand = new RelayCommand(EditReturnItem);
            DeleteReturnItemCommand = new RelayCommand(DeleteReturnItem);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            SoldItemDoubleClickCommand = new RelayCommand(SetProduct, obj => obj != null);
            ReturnCommand = new RelayCommand(ReturnItemCommand, IsReturnPossible);
        }

        private bool IsReturnPossible(object parameter)
        {
            if ((SelectedProduct != null || SelectedReturnProduct != null) && ReturnQty > 0 && ReturnQty <= soldQty)
            {
                return true;
            }
            return false;
        }

        private void ReturnItemCommand(object obj)
        {
            if (SelectedReturnProduct != null)
            {
                SelectedReturnProduct.ReturnQty = ReturnQty;
                SelectedReturnProduct.ReturnAmount = ReturnAmountForProduct;
                ObservableCollection<ReturnProduct> temp = new ObservableCollection<ReturnProduct>(ReturnItems);
                ReturnItems.Clear();
                ReturnItems = temp;
                SelectedProduct = null;
            }
            else
            {
                ReturnProduct returnProduct = new ReturnProduct
                {
                    Product = SelectedProduct,
                    ProductId = SelectedProduct.ProductId,
                    ReturnAmount = ReturnAmountForProduct,
                    ReturnQty = ReturnQty,
                    SaleDetailId = selectedSaleDetail.SaleDetailId,
                    BatchNumber = BatchNumber,
                    SoldAmount = selectedSaleDetail.UnitPrice,
                    SoldQty = selectedSaleDetail.QuantitySold
                };
                if (returnDetail == null)
                    returnDetail = new ReturnDetail();
                returnDetail.ReturnProducts.Add(returnProduct);
                ReturnItems.Add(returnProduct);
                SelectedProduct = null;
            }

            SelectedProductName = string.Empty;
            BatchNumber = string.Empty;
            UnitPrice = 0;
            ReturnQty = 0;
            TotalReturnAmount = ReturnItems.Sum(rp => rp.ReturnAmount);
            soldQty = 0;
            SaleAmount = 0;
            SoldQty = 0;
        }

        private void SetProduct(object obj)
        {
            if (obj != null)
            {
                if (obj is SaleDetail solditem)
                {
                    selectedSaleDetail = solditem;
                    SelectedProduct = solditem.Product;
                    SelectedProductName = SelectedProduct.ProductName;
                    BatchNumber = solditem.BatchNumber;
                    UnitPrice = solditem.UnitPrice;
                    SaleAmount = solditem.TotalPrice ?? 0;
                    SoldQty = solditem.QuantitySold;
                }
            }
        }

        private void EditReturnItem(object obj)
        {
            if (obj is ReturnProduct item)
            {
                SelectedReturnProduct = item;
                SelectedProductName = item.Product.ProductName;
                BatchNumber = item.BatchNumber;
                SoldQty = item.SoldQty;
                ReturnQty = item.ReturnQty;
                UnitPrice = item.SoldAmount;
                ReturnAmountForProduct = item.ReturnAmount;
                SaleAmount = item.SoldAmount;
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
                SaleDetail saleDetail = sale.SaleDetails.FirstOrDefault(x => x.ProductId == returnProduct.ProductId);
                if (saleDetail != null)
                {
                    saleDetail.QuantitySold = saleDetail.QuantitySold - returnProduct.ReturnQty;
                    saleDetail.TotalPrice = saleDetail.TotalPrice - returnProduct.ReturnAmount;
                    appDbContext.SaleDetails.Update(saleDetail);
                }
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
            ReturnAmountForProduct = ReturnQty * UnitPrice;
        }
    }
}
