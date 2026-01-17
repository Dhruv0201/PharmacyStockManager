using PharmacyStockManager.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    public class AddEditSalePaymentViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly AppDbContext _context = new AppDbContext();
        public event Action CloseWindow;

        public ObservableCollection<UserAccount> Users { get; set; }
        public ObservableCollection<String> PaymentModes { get; set; }

        private SalePayment salePayment;
        public SalePayment SalePayment
        {
            get => salePayment;
            set
            {
                salePayment = value;
                OnPropertyChanged(nameof(SalePayment));
            }
        }

        private DateTime? _paymentDate = DateTime.Now;
        public DateTime? PaymentDate
        {
            get => _paymentDate;
            set
            {
                _paymentDate = value;
                OnPropertyChanged(nameof(PaymentDate));
            }
        }

        private string _paymentMode;
        public string PaymentMode
        {
            get => _paymentMode;
            set
            {
                _paymentMode = value;
                OnPropertyChanged(nameof(PaymentMode));
            }
        }

        private decimal AmmountAlreadyPaid;
        private decimal _amountPaid;
        public decimal AmountPaid
        {
            get => _amountPaid;
            set
            {
                _amountPaid = value;
                OnPropertyChanged(nameof(AmountPaid));
                RecalculateDue();
            }
        }

        private decimal? _dueAmount;
        public decimal? DueAmount
        {
            get => _dueAmount;
            set
            {
                _dueAmount = value;
                OnPropertyChanged(nameof(DueAmount));
            }
        }

        private UserAccount _selectedCollectedBy;
        public UserAccount SelectedCollectedBy
        {
            get => _selectedCollectedBy;
            set
            {
                _selectedCollectedBy = value;
                OnPropertyChanged(nameof(SelectedCollectedBy));
            }
        }

        private decimal _totalBillAmount;
        public decimal TotalBillAmount
        {
            get => _totalBillAmount;
            set
            {
                _totalBillAmount = value;
                OnPropertyChanged(nameof(TotalBillAmount));
                RecalculateDue();
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
                    case nameof(PaymentDate):
                        if (PaymentDate == null)
                            return "Payment date is required.";
                        break;

                    case nameof(PaymentMode):
                        if (string.IsNullOrWhiteSpace(PaymentMode))
                            return "Payment mode is required.";
                        break;

                    case nameof(AmountPaid):
                        if (AmountPaid <= 0)
                            return "Amount paid must be greater than zero.";
                        break;

                    case nameof(SelectedCollectedBy):
                        if (SelectedCollectedBy == null)
                            return "Collected by is required.";
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

        public AddEditSalePaymentViewModel(decimal totalAmmount, decimal paidAmmount)
        {
            AmmountAlreadyPaid = paidAmmount;
            TotalBillAmount = totalAmmount;
            DueAmount = totalAmmount - paidAmmount;
            LoadUsers();
            PaymentModes = new ObservableCollection<string>
            {
                "Cash",
                "Card",
                "UPI",
                "Bank Transfer",
                "cheque"
            };
            SaveCommand = new RelayCommand(SavePayment, obj => true);
            CancelCommand = new RelayCommand(obj => CloseWindow?.Invoke(), obj => true);
        }

        public AddEditSalePaymentViewModel(SalePayment payment, decimal totalAmmount, decimal paidAmmount) : this(totalAmmount, paidAmmount)
        {
            SalePayment = payment;

            if (SalePayment != null)
            {
                PaymentDate = SalePayment.PaymentDate;
                PaymentMode = SalePayment.PaymentMode;
                AmountPaid = SalePayment.AmountPaid;
                DueAmount = SalePayment.DueAmount;

                SelectedCollectedBy = Users.FirstOrDefault(u => u.UserID == SalePayment.CollectedBy);
            }
        }

        private void LoadUsers()
        {
            Users = new ObservableCollection<UserAccount>(
                _context.UserAccounts.OrderBy(u => u.Username).ToList());
        }

        private void RecalculateDue()
        {
            DueAmount = TotalBillAmount - AmmountAlreadyPaid - AmountPaid;
        }

        private void SavePayment(object obj)
        {
            isValidationOn = true;

            if (HasErrors)
                return;

            if (SalePayment != null)
            {
                SalePayment.PaymentDate = PaymentDate;
                SalePayment.PaymentMode = PaymentMode;
                SalePayment.AmountPaid = AmountPaid;
                SalePayment.DueAmount = DueAmount;
                SalePayment.CollectedBy = SelectedCollectedBy?.UserID;
                SalePayment.ModifiedAt = DateTime.Now;
                SalePayment.ModifiedBy = App.LoggedInUser.UserID;
            }
            else
            {
                SalePayment = new SalePayment()
                {
                    PaymentDate = PaymentDate,
                    PaymentMode = PaymentMode,
                    AmountPaid = AmountPaid,
                    DueAmount = DueAmount,
                    CollectedBy = SelectedCollectedBy?.UserID,
                    CollectedByNavigation = SelectedCollectedBy
                };
            }

            CloseWindow?.Invoke();
        }
    }
}
