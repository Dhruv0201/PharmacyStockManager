using PharmacyStockManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    internal class AddEditProductViewModel : ViewModelBase,IDataErrorInfo
    {
        private readonly AppDbContext _context = new AppDbContext();
        private ObservableCollection<Category> _categories;
        public event Action CloseWindow;
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }
        private ObservableCollection<Supplier> _suppliers;
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set
            {
                _suppliers = value;
                OnPropertyChanged(nameof(Suppliers));
            }
        }

        private Product product;
        public Product Product
        {
            get => product;
            set
            {
                product = value;
                OnPropertyChanged(nameof(Product));
            }
        }

        private Supplier _selectedSupplier;
        public Supplier SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                _selectedSupplier = value;
                OnPropertyChanged(nameof(SelectedSupplier));
            }
        }

        private string _prductName;
        public string ProductName
        {
            get => _prductName;
            set
            {
                _prductName = value;
                OnPropertyChanged(nameof(ProductName));
            }
        }
        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
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

        private DateTime _expiryDate = DateTime.Now;
        public DateTime ExpiryDate
        {
            get => _expiryDate;
            set
            {
                _expiryDate = value;
                OnPropertyChanged(nameof(ExpiryDate));
            }
        }

        private decimal _purchasePrice;
        public decimal PurchasePrice
        {
            get => _purchasePrice;
            set
            {
                _purchasePrice = value;
                OnPropertyChanged(nameof(PurchasePrice));
            }
        }
        private decimal _sellingPrice;
        public decimal SellingPrice
        {
            get => _sellingPrice;
            set
            {
                _sellingPrice = value;
                OnPropertyChanged(nameof(SellingPrice));
            }
        }
        private int _quantityInStock;
        public int QuantityInStock
        {
            get => _quantityInStock;
            set
            {
                _quantityInStock = value;
                OnPropertyChanged(nameof(QuantityInStock));
            }
        }
        private int _reorderLevel;
        public int ReorderLevel
        {
            get => _reorderLevel;
            set
            {
                _reorderLevel = value;
                OnPropertyChanged(nameof(ReorderLevel));
            }
        }
        private bool isValidationOn;


        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if (!isValidationOn)
                {
                    return null;
                }
                switch (columnName)
                {
                    case nameof(ProductName):
                        if (string.IsNullOrEmpty(ProductName))
                            return "Product Name is required.";
                        break;
                    case nameof(SelectedCategory):
                        if (SelectedCategory == null)
                            return "Category is required.";
                        break;
                    case nameof(PurchasePrice):
                        if (PurchasePrice <= 0)
                            return "Purchase Price must be greater than zero.";
                        break;
                    case nameof(SellingPrice):
                        if (SellingPrice <= 0)
                            return "Selling Price must be greater than zero.";
                        break;
                    case nameof(ExpiryDate):
                        if (ExpiryDate != null && ExpiryDate <= (DateTime.Now))
                            return "Expiry Date must be a future date.";
                        break;
                    case nameof(BatchNumber):
                        if (string.IsNullOrEmpty(BatchNumber))
                            return "Please add some value for batch number";
                        break;
                    case nameof(SelectedSupplier):
                        if (SelectedSupplier == null)
                            return "Supplier is required.";
                        break;
                }
                return string.Empty;
            }
        }
        public bool HasErrors
        {
            get
            {
                OnPropertyChanged(null); // Notify that all properties should be re-validated
                var properties = GetType().GetProperties();
                foreach (var prop in properties)
                {
                    string error = this[prop.Name];  // calls IDataErrorInfo indexer
                    if (!string.IsNullOrEmpty(error))
                        return true;
                }
                return false;
            }
        }   
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private void SaveProduct(object obj)
        {
            isValidationOn = true;
            if (HasErrors)
                return;
            else
            {
                if (product != null)
                {
                    product.ProductName = ProductName;
                    product.PurchasePrice = PurchasePrice;
                    product.SellingPrice = SellingPrice;
                    product.BatchNumber = BatchNumber;
                    product.ExpiryDate = DateOnly.FromDateTime(ExpiryDate);
                    product.CategoryId = SelectedCategory.CategoryId;
                    product.SupplierId = SelectedSupplier.SupplierId;
                    product.CreatedAt = DateTime.Now;
                    product.QuantityInStock = QuantityInStock;
                    product.ReorderLevel = ReorderLevel;
                    product.ModifiedAt = DateTime.Now;
                    product.ModifiedBy = App.LoggedInUser.UserId;
                }
                else
                {
                    product = new Product();
                    product.ProductName = ProductName;
                    product.PurchasePrice = PurchasePrice;
                    product.SellingPrice = SellingPrice;
                    product.BatchNumber = BatchNumber;
                    product.ExpiryDate = DateOnly.FromDateTime(ExpiryDate);
                    product.CategoryId = SelectedCategory.CategoryId;
                    product.SupplierId = SelectedSupplier.SupplierId;
                    product.CreatedAt = DateTime.Now;
                    product.QuantityInStock = QuantityInStock;
                    product.ReorderLevel = ReorderLevel;
                    _context.Products.Add(product);
                }
                _context.SaveChanges();

            }
            CloseWindow?.Invoke();
        }

        public  AddEditProductViewModel()
        {
            BindCategoriesAndProducts();
            SaveCommand = new RelayCommand(SaveProduct, obj => true);
            CancelCommand = new RelayCommand(obj => CloseWindow?.Invoke(), obj => true);
        }
        public AddEditProductViewModel(int productId) :this()
        {
            product = _context.Products.Find(productId);
            if (product != null)
            {
                ProductName = product.ProductName;
                PurchasePrice = product.PurchasePrice;
                SellingPrice = product.SellingPrice;
                BatchNumber = product.BatchNumber;
                ExpiryDate = product.ExpiryDate.HasValue ? product.ExpiryDate.Value.ToDateTime(new TimeOnly(0, 0)) : DateTime.Now;
                SelectedCategory = Categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
                SelectedSupplier = Suppliers.FirstOrDefault(s => s.SupplierId == product.SupplierId);
                QuantityInStock = product.QuantityInStock;
                ReorderLevel = product.ReorderLevel;
            }
        }

        private void BindCategoriesAndProducts()
        {
            try
            {
                Categories = new ObservableCollection<Category>(_context.Categories.OrderBy(c => c.CategoryName).ToList());
                Suppliers = new ObservableCollection<Supplier>(_context.Suppliers.OrderBy(s => s.SupplierName).ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

       
    }
}
