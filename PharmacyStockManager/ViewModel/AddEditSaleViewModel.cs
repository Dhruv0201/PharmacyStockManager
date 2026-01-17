using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using PharmacyStockManager.Views;
using PharmacyStockManager.Views.PopupWindows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    internal class AddEditSaleViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly AppDbContext _context = new AppDbContext();
        public event Action CloseWindow;

        public ObservableCollection<Customer> Customers { get; set; }
        public ObservableCollection<UserAccount> Users { get; set; }
        public ObservableCollection<SaleDetail> SaleDetails { get; set; } = new();
        public ObservableCollection<SalePayment> SalePayments { get; set; } = new();

        public SaleDetail SelectedSaleDetail { get; set; }
        public SalePayment SelectedPayment { get; set; }

        private Sale sale;
        public Sale Sale
        {
            get => sale;
            set
            {
                sale = value;
                OnPropertyChanged(nameof(Sale));
            }
        }

        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged(nameof(SelectedCustomer));
            }
        }

        private UserAccount _selectedSoldBy;
        public UserAccount SelectedSoldBy
        {
            get => _selectedSoldBy;
            set
            {
                _selectedSoldBy = value;
                OnPropertyChanged(nameof(SelectedSoldBy));
            }
        }

        private DateTime? _saleDate = DateTime.Now;
        public DateTime? SaleDate
        {
            get => _saleDate;
            set
            {
                _saleDate = value;
                OnPropertyChanged(nameof(SaleDate));
            }
        }

        private decimal _totalAmount;
        public decimal TotalAmount
        {
            get => _totalAmount;
            set
            {
                _totalAmount = value;
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        private decimal _totalPaid;
        public decimal TotalPaid
        {
            get => _totalPaid;
            set
            {
                _totalPaid = value;
                OnPropertyChanged(nameof(TotalPaid));
            }
        }

        private decimal _dueAmount;
        public decimal DueAmount
        {
            get => _dueAmount;
            set
            {
                _dueAmount = value;
                OnPropertyChanged(nameof(DueAmount));
            }
        }

        private bool isValidationOn = false;

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if (!isValidationOn)
                    return null;

                switch (columnName)
                {
                    case nameof(SelectedCustomer):
                        if (SelectedCustomer == null)
                            return "Customer is required.";
                        break;

                    case nameof(SaleDate):
                        if (SaleDate == null)
                            return "Sale Date is required.";
                        break;

                    case nameof(SelectedSoldBy):
                        if (SelectedSoldBy == null)
                            return "Sold By is required.";
                        break;

                    case nameof(TotalAmount):
                        if (TotalAmount <= 0)
                            return "Total Amount must be greater than zero.";
                        break;
                }

                return string.Empty;
            }
        }

        public bool HasErrors
        {
            get
            {
                OnPropertyChanged(null);
                var props = GetType().GetProperties();
                foreach (var p in props)
                {
                    if (!string.IsNullOrEmpty(this[p.Name]))
                        return true;
                }
                return false;
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ICommand AddItemCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand DeleteItemCommand { get; }

        public ICommand AddPaymentCommand { get; }
        public ICommand EditPaymentCommand { get; }
        public ICommand DeletePaymentCommand { get; }

        public ICommand SelectionSaleDetail { get; }
        public ICommand SelectionPaymentDetail { get; }

        public AddEditSaleViewModel()
        {
            BindDropdowns();

            SaveCommand = new RelayCommand(SaveSale, obj => true);
            CancelCommand = new RelayCommand(obj => CloseWindow?.Invoke(), obj => true);

            AddItemCommand = new RelayCommand(AddItem);
            EditItemCommand = new RelayCommand(EditItem);
            DeleteItemCommand = new RelayCommand(DeleteItem);

            AddPaymentCommand = new RelayCommand(AddPayment, (obj) => TotalAmount != 0);
            EditPaymentCommand = new RelayCommand(EditPayment);
            DeletePaymentCommand = new RelayCommand(DeletePayment);
        }

        public AddEditSaleViewModel(int SaleId) : this()
        {
            sale = _context.Sales
                .Include(s => s.SaleDetails).ThenInclude(x=>x.Product)
                .Include(s => s.SalePayments)
                .FirstOrDefault(s => s.SaleId == SaleId);

            if (sale != null)
            {
                SelectedCustomer = Customers.FirstOrDefault(c => c.CustomerId == sale.CustomerId);
                SelectedSoldBy = Users.FirstOrDefault(u => u.UserId == sale.SoldBy);

                SaleDate = sale.SaleDate;
                TotalAmount = sale.TotalAmount ?? 0;

                SaleDetails = new ObservableCollection<SaleDetail>(sale.SaleDetails);
                SalePayments = new ObservableCollection<SalePayment>(sale.SalePayments);
                RecalculateDue();
            }
        }

        private void BindDropdowns()
        {
            Customers = new ObservableCollection<Customer>(_context.Customers.OrderBy(c => c.Name).ToList());
            Users = new ObservableCollection<UserAccount>(_context.UserAccounts.OrderBy(u => u.Username).ToList());
        }

        private void AddItem(object obj)
        {
            var dialog = new SaleDetailDialog();
            var productWindow = Application.Current.MainWindow as MainWindow;

            dialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];
            productWindow?.RootLayout.Children.Add(dialog);

            dialog.Closed += delegate
            {
                if (dialog.DialogResult == true)
                {
                    SaleDetails.Add(dialog.ViewModel.SaleDetail);
                    RecalculateTotal();
                    RecalculateDue();
                }
                productWindow.RootLayout.Children.Remove(dialog);
            };

            dialog.Show();
        }

        private void EditItem(object obj)
        {
            if (obj != null)
                SelectedSaleDetail = obj as SaleDetail;
            if (SelectedSaleDetail == null)
                return;

            var dialog = new SaleDetailDialog(SelectedSaleDetail);
            var main = Application.Current.MainWindow as MainWindow;

            dialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];
            main?.RootLayout.Children.Add(dialog);

            dialog.Closed += delegate
            {
                if (dialog.DialogResult == true)
                {
                    var updated = dialog.ViewModel.SaleDetail;
                    var index = SaleDetails.IndexOf(SelectedSaleDetail);
                    SaleDetails.RemoveAt(index);
                    SaleDetails.Insert(index, updated);
                    RecalculateTotal();
                    RecalculateDue();
                }
                main.RootLayout.Children.Remove(dialog);
            };

            dialog.Show();
        }

        private void DeleteItem(object obj)
        {
            if (obj != null)
                SelectedSaleDetail = obj as SaleDetail;
            if (SelectedSaleDetail != null)
            {
                SaleDetails.Remove(SelectedSaleDetail);
                RecalculateTotal();
                RecalculateDue();
                SelectedSaleDetail = null;
            }
        }

        private void AddPayment(object obj)
        {
            decimal totalAmount = SaleDetails.Sum(s => s.TotalPrice ?? 0);
            decimal paidAmount = SalePayments.Sum(p => p.AmountPaid);
            var dialog = new SalePaymentDialog(totalAmount, paidAmount);
            var main = Application.Current.MainWindow as MainWindow;

            dialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];
            main?.RootLayout.Children.Add(dialog);

            dialog.Closed += delegate
            {
                if (dialog.DialogResult == true)
                {
                    SalePayments.Add(dialog.ViewModel.SalePayment);
                    RecalculateDue();
                }
                main.RootLayout.Children.Remove(dialog);
            };

            dialog.Show();
        }

        private void EditPayment(object obj)
        {
            if (obj != null)
                SelectedPayment = obj as SalePayment;
            if (SelectedPayment == null)
                return;

            decimal totalAmount = SaleDetails.Sum(s => s.TotalPrice ?? 0);
            decimal paidAmount = SalePayments.Sum(p => p.AmountPaid) - SelectedPayment.AmountPaid;
            var dialog = new SalePaymentDialog(SelectedPayment, totalAmount, paidAmount);
            var main = Application.Current.MainWindow as MainWindow;

            dialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];
            main?.RootLayout.Children.Add(dialog);

            dialog.Closed += delegate
            {
                if (dialog.DialogResult == true)
                {
                    var updated = dialog.ViewModel.SalePayment;
                    var index = SalePayments.IndexOf(SelectedPayment);
                    SalePayments.RemoveAt(index);
                    SalePayments.Insert(index, updated);
                    RecalculateDue();
                }
                main.RootLayout.Children.Remove(dialog);
            };

            dialog.Show();
        }

        private void DeletePayment(object obj)
        {
            if (obj != null)
                SelectedPayment = obj as SalePayment;
            if (SelectedPayment != null)
            {
                SalePayments.Remove(SelectedPayment);
                RecalculateDue();
                SelectedPayment = null;
            }
        }

        private void RecalculateTotal()
        {
            TotalAmount = SaleDetails.Sum(d => d.TotalPrice ?? (d.UnitPrice * d.QuantitySold));
        }

        private void RecalculateDue()
        {
            decimal billAmount = SaleDetails?.Sum(s => s?.TotalPrice ?? 0) ?? 0;
            decimal paidAmount = SalePayments?.Sum(p => p?.AmountPaid ?? 0) ?? 0;

            TotalPaid = paidAmount;
            DueAmount = billAmount - paidAmount;
        }


        private void SaveSale(object obj)
        {
            isValidationOn = true;
            if (HasErrors)
                return;

            if (sale != null)
            {
                sale.CustomerId = SelectedCustomer.CustomerId;
                sale.SaleDate = SaleDate;
                sale.SoldBy = SelectedSoldBy?.UserId;
                sale.TotalAmount = TotalAmount;
                sale.ModifiedAt = DateTime.Now;
                sale.ModifiedBy = App.LoggedInUser.UserId;

                // Clear and add fresh clean entities
                sale.SaleDetails.Clear();
                foreach (var d in SaleDetails)
                {
                    sale.SaleDetails.Add(new SaleDetail
                    {
                        ProductId = d.ProductId,
                        BatchNumber = d.BatchNumber,
                        QuantitySold = d.QuantitySold,
                        UnitPrice = d.UnitPrice,
                        TotalPrice = d.TotalPrice,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = App.LoggedInUser.UserId
                    });
                }

                sale.SalePayments.Clear();
                foreach (var p in SalePayments)
                {
                    sale.SalePayments.Add(new SalePayment
                    {
                        PaymentDate = p.PaymentDate,
                        PaymentMode = p.PaymentMode,
                        AmountPaid = p.AmountPaid,
                        DueAmount = p.DueAmount,
                        CollectedBy = p.CollectedBy,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = App.LoggedInUser.UserId
                    });
                }
            }
            else
            {
                sale = new Sale()
                {
                    CustomerId = SelectedCustomer.CustomerId,
                    SaleDate = SaleDate,
                    SoldBy = SelectedSoldBy?.UserId,
                    TotalAmount = TotalAmount
                };

                _context.Sales.Add(sale);

                foreach (var d in SaleDetails)
                {
                    sale.SaleDetails.Add(new SaleDetail
                    {
                        ProductId = d.ProductId,
                        BatchNumber = d.BatchNumber,
                        QuantitySold = d.QuantitySold,
                        UnitPrice = d.UnitPrice,
                        TotalPrice = d.TotalPrice,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = App.LoggedInUser.UserId
                    });
                }

                foreach (var p in SalePayments)
                {
                    sale.SalePayments.Add(new SalePayment
                    {
                        PaymentDate = p.PaymentDate,
                        PaymentMode = p.PaymentMode,
                        AmountPaid = p.AmountPaid,
                        DueAmount = p.DueAmount,
                        CollectedBy = p.CollectedBy,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = App.LoggedInUser.UserId
                    });
                }
            }

            _context.SaveChanges();
            CloseWindow?.Invoke();

        }
    }
}
