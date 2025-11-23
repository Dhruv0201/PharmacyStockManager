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
    public class PurchaseReturnViewModel : ViewModelBase
    {
        private readonly AppDbContext _context = new AppDbContext();

        private ObservableCollection<PurchaseReturnDetail> _purchaseReturns = new();
        public ObservableCollection<PurchaseReturnDetail> PurchaseReturns
        {
            get => _purchaseReturns;
            set
            {
                _purchaseReturns = value;
                OnPropertyChanged(nameof(PurchaseReturns));
            }
        }

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

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                LoadPurchaseReturns(_searchText);
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        public PurchaseReturnViewModel()
        {
            AddCommand = new RelayCommand(_ => AddPurchaseReturn());
            EditCommand = new RelayCommand(obj => EditPurchaseReturn(obj as PurchaseReturnDetail));
            DeleteCommand = new RelayCommand(obj => DeletePurchaseReturn(obj as PurchaseReturnDetail));
            RefreshCommand = new RelayCommand(_ => LoadPurchaseReturns());

            LoadProducts();
            LoadPurchaseReturns();
        }

        private void LoadProducts()
        {
            Products.Clear();
            Products = new ObservableCollection<Product>(
                _context.Products
                .AsNoTracking()
                .OrderBy(p => p.ProductName)
                .ToList()
            );
        }

        public void LoadPurchaseReturns(string? filter = null)
        {
            PurchaseReturns.Clear();

            var query = _context.PurchaseReturnDetails
                .AsNoTracking()
                .Include(p => p.Product)
                .Include(p => p.PurchaseReturn)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x =>
                    x.Product.ProductName.Contains(filter) ||
                    x.BatchNumber.Contains(filter)
                );
            }

            PurchaseReturns = new ObservableCollection<PurchaseReturnDetail>(
                query.OrderByDescending(x => x.PurchaseReturnDetailId).ToList()
            );
        }

        private void AddPurchaseReturn()
        {
            MainWindow main = Application.Current.MainWindow as MainWindow;

            PurchaseReturnDetailDialog dialog = new PurchaseReturnDetailDialog();
            dialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];

            main?.RootLayout.Children.Add(dialog);

            dialog.Closed += delegate
            {
                LoadPurchaseReturns(SearchText);
                main.RootLayout.Children.Remove(dialog);
            };

            dialog.Show();
        }

        private void EditPurchaseReturn(PurchaseReturnDetail detail)
        {
            if (detail == null) return;

            MainWindow main = Application.Current.MainWindow as MainWindow;

            PurchaseReturnDetailDialog dialog = new PurchaseReturnDetailDialog(detail.PurchaseReturnDetailId);
            dialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];

            main?.RootLayout.Children.Add(dialog);

            dialog.Closed += delegate
            {
                LoadPurchaseReturns(SearchText);
                main.RootLayout.Children.Remove(dialog);
            };

            dialog.Show();
        }

        private void DeletePurchaseReturn(PurchaseReturnDetail detail)
        {
            if (detail == null) return;

            if (MessageBox.Show($"Delete return entry for '{detail.Product?.ProductName}'?",
                                "Confirm Delete",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning)
                == MessageBoxResult.Yes)
            {
                _context.PurchaseReturnDetails.Remove(detail);
                _context.SaveChanges();
                LoadPurchaseReturns(SearchText);
            }
        }
    }
}
