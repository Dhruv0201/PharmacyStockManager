using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using PharmacyStockManager.Views;
using PharmacyStockManager.Views.PopupWindows;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    public class ProductsViewModel : ViewModelBase
    {
        private readonly AppDbContext _context = new AppDbContext();
        public event Action AddPordut;

        private ObservableCollection<Product> _products = new();
        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        private ObservableCollection<Category> _categories = new();
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        private ObservableCollection<Supplier> _suppliers = new();
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set
            {
                _suppliers = value;
                OnPropertyChanged(nameof(Suppliers));
            }
        }


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

        public ProductsViewModel()
        {
            AddCommand = new RelayCommand(_ => AddProduct());
            EditCommand = new RelayCommand(obj => EditProduct(obj as Product));
            DeleteCommand = new RelayCommand(obj => DeleteProduct(obj as Product));
            RefreshCommand = new RelayCommand(_ => LoadProducts());

            LoadCategoriesAndSuppliers();
            LoadProducts();
        }

        private void LoadCategoriesAndSuppliers()
        {
            Categories.Clear();
            Suppliers.Clear();
            Categories = new ObservableCollection<Category>(_context.Categories.AsNoTracking().OrderBy(c => c.CategoryName).ToList());
            Suppliers = new ObservableCollection<Supplier>(_context.Suppliers.AsNoTracking().OrderBy(s => s.SupplierName).ToList());
        }

        public void LoadProducts(string? filter = null)
        {
            Products.Clear();
            Products = new ObservableCollection<Product> (_context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => string.IsNullOrEmpty(filter) || p.ProductName.Contains(filter))
                .OrderBy(p => p.ProductName).ToList());
        }

        private void AddProduct()
        {
            MainWindow main = Application.Current.MainWindow as MainWindow;
            ProductDialog productDialog = new ProductDialog();
            productDialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];
            main?.RootLayout.Children.Add(productDialog);
            productDialog.Closed += delegate {
                LoadProducts(SearchText);
                main.RootLayout.Children.Remove(productDialog);
            };
            productDialog.Show();
        }

        private void EditProduct(Product product)
        {

            if (product == null) return;

            MainWindow main = Application.Current.MainWindow as MainWindow;
            ProductDialog productDialog = new ProductDialog(product.ProductId);
            productDialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];
            main?.RootLayout.Children.Add(productDialog);
            productDialog.Closed += delegate { LoadProducts(SearchText);
                main.RootLayout.Children.Remove(productDialog);
            };    
            productDialog.Show();

        }


        private void DeleteProduct(Product product)
        {
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
