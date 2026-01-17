using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using PharmacyStockManager.Views;
using PharmacyStockManager.Views.PopupWindows;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    public class SalesViewModel : ViewModelBase
    {
        private readonly AppDbContext _context = new AppDbContext();

        private ObservableCollection<Sale> _sales = new();
        public ObservableCollection<Sale> Sales
        {
            get => _sales;
            set
            {
                _sales = value;
                OnPropertyChanged(nameof(Sales));
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
                LoadSales(_searchText);
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SaleReturnCommand { get; }

        public SalesViewModel()
        {
            AddCommand = new RelayCommand(_ => AddSale());
            EditCommand = new RelayCommand(obj => EditSale(obj as Sale));
            DeleteCommand = new RelayCommand(obj => DeleteSale(obj as Sale));
            RefreshCommand = new RelayCommand(_ => LoadSales());
            SaleReturnCommand = new RelayCommand(obj => ViewSale(obj as Sale));

            LoadSales();
        }

        public void LoadSales(string filter = null)
        {
            var query = _context.Sales
                .AsNoTracking()
                .Include(s => s.Customer)
                .Include(s => s.SoldByNavigation)
                .Include(s => s.SalePayments)
                .Include(s => s.SaleDetails)
                .OrderByDescending(s => s.SaleDate)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(s =>
                    (s.Customer.Name ?? "").Contains(filter) ||
                    (s.SoldByNavigation.Username ?? "").Contains(filter) ||
                    s.SaleId.ToString().Contains(filter)
                );
            }

            var salesList = query.ToList();

            foreach (var s in salesList)
            {
                var paid = s.SalePayments.Sum(p => p.AmountPaid);
                var total = s.TotalAmount ?? 0;

                s.PaymentStatus = paid >= total ? "Paid" : $"Due: {total - paid}";
            }

            Sales = new ObservableCollection<Sale>(salesList);
        }


        private void AddSale()
        {
            MainWindow main = Application.Current.MainWindow as MainWindow;
            SaleDialog dlg = new SaleDialog();
            dlg.Style = (Style)Application.Current.Resources["ChildWindowStyle"];

            main?.RootLayout.Children.Add(dlg);

            dlg.Closed += delegate
            {
                LoadSales(SearchText);
                main.RootLayout.Children.Remove(dlg);
            };

            dlg.Show();
        }

        private void EditSale(Sale sale)
        {
            if (sale == null) return;

            MainWindow main = Application.Current.MainWindow as MainWindow;
            SaleDialog dlg = new SaleDialog(sale.SaleId);
            dlg.Style = (Style)Application.Current.Resources["ChildWindowStyle"];

            main?.RootLayout.Children.Add(dlg);

            dlg.Closed += delegate
            {
                LoadSales(SearchText);
                main.RootLayout.Children.Remove(dlg);
            };

            dlg.Show();
        }

        private void ViewSale(Sale sale)
        {

            var dialog = new SalesReturnDialog(sale.SaleId);

            MainWindow main = Application.Current.MainWindow as MainWindow;
            dialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];

            main?.RootLayout.Children.Add(dialog);
            // attach childwindow style like before
            dialog.Show();
        }

        private void DeleteSale(Sale sale)
        {
            if (sale == null) return;

            if (MessageBox.Show(
                $"Delete sale for '{sale.Customer?.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _context.Sales.Remove(sale);
                _context.SaveChanges();
                LoadSales(SearchText);
            }
        }
    }
}
