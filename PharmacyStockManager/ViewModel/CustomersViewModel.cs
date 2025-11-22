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
    public class CustomersViewModel : ViewModelBase
    {
        private readonly AppDbContext _context = new AppDbContext();

        private ObservableCollection<Customer> _customers = new();
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set
            {
                _customers = value;
                OnPropertyChanged(nameof(Customers));
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
                LoadCustomers(_searchText);
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        public CustomersViewModel()
        {

            AddCommand = new RelayCommand(_ => AddCustomer());
            EditCommand = new RelayCommand(obj => EditCustomer(obj as Customer));
            DeleteCommand = new RelayCommand(obj => DeleteCustomer(obj as Customer));
            RefreshCommand = new RelayCommand(_ => LoadCustomers());

            LoadCustomers();
        }

        public void LoadCustomers(string? filter = null)
        {
            Customers.Clear();
            Customers = new ObservableCollection<Customer>(_context.Customers.AsNoTracking()
                .Where(c => string.IsNullOrEmpty(filter) || c.Name.Contains(filter))
                .OrderBy(c => c.Name)
                .ToList());
        }

        private void AddCustomer()
        {
            MainWindow main = Application.Current.MainWindow as MainWindow;

            CustomerDialog dialog = new CustomerDialog();
            dialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];
            main.RootLayout.Children.Add(dialog);

            dialog.Closed += (s, e) =>
            {
                LoadCustomers(SearchText);
                main.RootLayout.Children.Remove(dialog);
            };

            dialog.Show();

        }

        private void EditCustomer(Customer customer)
        {
            if(customer == null) return;
            MainWindow main = Application.Current.MainWindow as MainWindow;

            CustomerDialog dialog = new CustomerDialog(customer.CustomerId);
            dialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];
            main.RootLayout.Children.Add(dialog);

            dialog.Closed += (s, e) =>
            {
                LoadCustomers(SearchText);
                main.RootLayout.Children.Remove(dialog);
            };

            dialog.Show();

        }

        private void DeleteCustomer(Customer customer)
        {
            if (customer == null) return;

            if (System.Windows.MessageBox.Show(
                $"Delete customer '{customer.Name}'?",
                "Confirm Delete",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.Yes)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
                LoadCustomers(SearchText);
            }
        }
    }
}
