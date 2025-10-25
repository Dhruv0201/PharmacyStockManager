using PharmacyStockManager.Models;
using PharmacyStockManager.Views.PopupWindows;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    public class ProductsViewModel : ViewModelBase
    {
        private readonly AppDbContext _context;

        public ObservableCollection<ProductItemViewModel> Products { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<Supplier> Suppliers { get; set; } = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                LoadProducts(_searchText);
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        public ProductsViewModel(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            AddCommand = new RelayCommand(_ => AddProduct());
            EditCommand = new RelayCommand(obj => EditProduct(obj as ProductItemViewModel));
            DeleteCommand = new RelayCommand(obj => DeleteProduct(obj as ProductItemViewModel));
            RefreshCommand = new RelayCommand(_ => LoadProducts());

            LoadCategoriesAndSuppliers();
            LoadProducts();
        }

        private void LoadCategoriesAndSuppliers()
        {
            Categories.Clear();
            Suppliers.Clear();

            foreach (var cat in _context.Categories.OrderBy(c => c.CategoryName))
                Categories.Add(cat);

            foreach (var sup in _context.Suppliers.OrderBy(s => s.SupplierName))
                Suppliers.Add(sup);
        }

        public void LoadProducts(string? filter = null)
        {
            Products.Clear();

            var categoriesDict = _context.Categories.ToDictionary(c => c.CategoryId, c => c.CategoryName);
            var suppliersDict = _context.Suppliers.ToDictionary(s => s.SupplierId, s => s.SupplierName);

            var products = _context.Products
                .Where(p => string.IsNullOrWhiteSpace(filter) || p.ProductName.Contains(filter))
                .OrderBy(p => p.ProductId)
                .AsEnumerable()
                .Select((p, index) => new ProductItemViewModel
                {
                    SerialNumber = index + 1,
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    CategoryName = p.CategoryId.HasValue && categoriesDict.TryGetValue(p.CategoryId.Value, out var cat) ? cat : "Unknown",
                    SupplierName = p.SupplierId.HasValue && suppliersDict.TryGetValue(p.SupplierId.Value, out var sup) ? sup : "Unknown",
                    BatchNumber = p.BatchNumber ?? "",
                    ExpiryDate = p.ExpiryDate,
                    PurchasePrice = p.PurchasePrice,
                    SellingPrice = p.SellingPrice,
                    QuantityInStock = p.QuantityInStock,
                    ReorderLevel = p.ReorderLevel
                })
                .ToList();

            foreach (var p in products)
                Products.Add(p);
        }

        private void AddProduct()
        {
            var dialog = new ProductDialog(Categories, Suppliers);

            if (dialog.ShowDialog() == true)
            {
                var newProduct = new Product
                {
                    ProductName = dialog.ProductName,
                    CategoryId = dialog.CategoryId,
                    SupplierId = dialog.SupplierId,
                    BatchNumber = dialog.BatchNumber,
                    ExpiryDate = dialog.ExpiryDate,
                    PurchasePrice = dialog.PurchasePrice,
                    SellingPrice = dialog.SellingPrice,
                    QuantityInStock = dialog.QuantityInStock,
                    ReorderLevel = dialog.ReorderLevel,
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = App.LoggedInUser.UserId
                };

                _context.Products.Add(newProduct);
                _context.SaveChanges();
                LoadProducts(SearchText);
            }
        }

        private void EditProduct(ProductItemViewModel? productVM)
        {
            if (productVM == null) return;

            var product = _context.Products.Find(productVM.ProductId);
            if (product == null) return;

            var dialog = new ProductDialog(Categories, Suppliers, product);

            if (dialog.ShowDialog() == true)
            {
                product.ProductName = dialog.ProductName;
                product.CategoryId = dialog.CategoryId;
                product.SupplierId = dialog.SupplierId;
                product.BatchNumber = dialog.BatchNumber;
                product.ExpiryDate = dialog.ExpiryDate;
                product.PurchasePrice = dialog.PurchasePrice;
                product.SellingPrice = dialog.SellingPrice;
                product.QuantityInStock = dialog.QuantityInStock;
                product.ReorderLevel = dialog.ReorderLevel;
                product.ModifiedAt = DateTime.Now;
                product.ModifiedBy = App.LoggedInUser.UserId;

                _context.SaveChanges();
                LoadProducts(SearchText);
            }
        }


        private void DeleteProduct(ProductItemViewModel? productVM)
        {
            if (productVM == null) return;

            var product = _context.Products.Find(productVM.ProductId);
            if (product == null) return;

            if (System.Windows.MessageBox.Show(
                $"Delete product '{product.ProductName}'?",
                "Confirm Delete",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.Yes)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
                LoadProducts(SearchText);
            }
        }
    }
}
