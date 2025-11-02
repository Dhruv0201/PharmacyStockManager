using PharmacyStockManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    internal class AddEditCustomerViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly AppDbContext _context = new AppDbContext();
        public event Action CloseWindow;
        private string _customerName = string.Empty;
        public string CustomerName
        {
            get => _customerName;
            set
            {
                _customerName = value;
                OnPropertyChanged(nameof(CustomerName));
            }
        }
        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }

        private Customer _customer;
        public Customer Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                if (_customer != null)
                {
                    CustomerName = _customer.Name;
                    PhoneNumber = _customer.PhoneNumber ?? string.Empty;
                }
                OnPropertyChanged(nameof(Customer));
            }
        }

        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }

        bool isValidationOn;
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if (!isValidationOn)
                    return null;

                return columnName switch
                {
                    nameof(CustomerName) when string.IsNullOrEmpty(CustomerName)
                        => "Customer name is required.",
                    nameof(PhoneNumber) when string.IsNullOrEmpty(PhoneNumber)
                        => "Phone number is required.",
                    _ => null
                };
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

        public AddEditCustomerViewModel()
        {
            SaveCommand = new RelayCommand(ExecuteSave, (obj) => true);
            CancelCommand = new RelayCommand((obj) => CloseWindow?.Invoke(), (obj) => true);
        }

        public AddEditCustomerViewModel(int customerId) : this()
        {
            Customer = _context.Customers.Find(customerId);
        }

        private void ExecuteSave(object obj)
        {
            isValidationOn = true;
            if (HasErrors)
                return;
            if (Customer == null)
            {
                var newCustomer = new Customer
                {
                    Name = this.CustomerName,
                    PhoneNumber = this.PhoneNumber,
                    CreatedAt = DateTime.Now
                };
                _context.Customers.Add(newCustomer);
            }
            else
            {
                // Update existing customer
                var existingCustomer = _context.Customers.Find(Customer.CustomerId);
                if (existingCustomer != null)
                {
                    existingCustomer.Name = this.CustomerName;
                    existingCustomer.PhoneNumber = this.PhoneNumber;
                    existingCustomer.ModifiedAt = DateTime.Now;
                }
            }
            _context.SaveChanges();
            isValidationOn = false;
            CloseWindow?.Invoke();
        }

        public AddEditCustomerViewModel(Customer customer) : this()
        {
            Customer = customer;
        }
    }
}
