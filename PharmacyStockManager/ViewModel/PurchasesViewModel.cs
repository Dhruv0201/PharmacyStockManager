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
    public class PurchasesViewModel : ViewModelBase
    {
        private readonly AppDbContext _context = new AppDbContext();

        private ObservableCollection<Purchase> _purchases = new();
        public ObservableCollection<Purchase> Purchases
        {
            get => _purchases;
            set
            {
                _purchases = value;
                OnPropertyChanged(nameof(Purchases));
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
                LoadPurchases(_searchText);
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ViewInvoiceCommand { get; }

        public PurchasesViewModel()
        {
            AddCommand = new RelayCommand(_ => AddPurchase());
            EditCommand = new RelayCommand(obj => EditPurchase(obj as Purchase));
            DeleteCommand = new RelayCommand(obj => DeletePurchase(obj as Purchase));
            RefreshCommand = new RelayCommand(_ => LoadPurchases());
            ViewInvoiceCommand = new RelayCommand(obj => ViewInvoice(obj as Purchase));

            LoadSuppliers();
            LoadPurchases();
        }

        private void LoadSuppliers()
        {
            Suppliers.Clear();
            Suppliers = new ObservableCollection<Supplier>(
                _context.Suppliers
                .AsNoTracking()
                .OrderBy(s => s.SupplierName)
                .ToList()
            );
        }

        public void LoadPurchases(string? filter = null)
        {
            Purchases.Clear();

            Purchases = new ObservableCollection<Purchase>(
                _context.Purchases
                .AsNoTracking()
                .Include(p => p.Supplier)
                .Include(p => p.PurchasedByNavigation)
                .Include(p => p.ModifiedByNavigation)
                .Where(p =>
                    string.IsNullOrEmpty(filter) ||
                    p.InvoiceNumber.Contains(filter) ||
                    (p.Supplier != null && p.Supplier.SupplierName.Contains(filter))
                )
                .OrderByDescending(p => p.PurchaseDate)
                .ToList()
            );
        }

        private void AddPurchase()
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            PurchaseDialog purchaseDialog = new PurchaseDialog();
            purchaseDialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];

            mainWindow?.RootLayout.Children.Add(purchaseDialog);
            purchaseDialog.Closed += delegate { LoadPurchases(SearchText);
                mainWindow.RootLayout.Children.Remove(purchaseDialog);
            };
            purchaseDialog.Show();
        }

        private void EditPurchase(Purchase purchase)
        {
            if (purchase == null) return;

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            PurchaseDialog purchaseDialog = new PurchaseDialog(purchase.PurchaseId);
            purchaseDialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];

            mainWindow?.RootLayout.Children.Add(purchaseDialog);
            purchaseDialog.Closed += delegate { LoadPurchases(SearchText);
                mainWindow.RootLayout.Children.Remove(purchaseDialog);
            };
            purchaseDialog.Show();
        }

        private void DeletePurchase(Purchase purchase)
        {
            if (purchase == null) return;

            if (MessageBox.Show(
                $"Delete purchase Invoice '{purchase.InvoiceNumber}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _context.Purchases.Remove(purchase);
                _context.SaveChanges();
                LoadPurchases(SearchText);
            }
        }

        private void ViewInvoice(Purchase purchase)
        {
            if (purchase == null || string.IsNullOrEmpty(purchase.InvoiceImagePath))
            {
                MessageBox.Show("No invoice file found.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string appFolder = AppDomain.CurrentDomain.BaseDirectory;
            string invoiceFolder = System.IO.Path.Combine(appFolder, "InvoiceFiles");

            string fullPath = System.IO.Path.Combine(invoiceFolder, purchase.InvoiceImagePath);

            if (!System.IO.File.Exists(fullPath))
            {
                MessageBox.Show("Invoice file is missing from the InvoiceFiles folder.",
                                "File Not Found",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = fullPath,
                UseShellExecute = true
            });
        }

    }
}
