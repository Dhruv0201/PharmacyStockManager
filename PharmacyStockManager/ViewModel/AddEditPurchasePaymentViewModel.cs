using PharmacyStockManager.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    public class AddEditPurchasePaymentViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly AppDbContext _context = new AppDbContext();
        public event Action CloseWindow;

        public ObservableCollection<UserAccount> Users { get; set; }
        public ObservableCollection<string> PaymentModes { get; set; }

        private PurchasePayment purchasePayment;
        public PurchasePayment PurchasePayment
        {
            get => purchasePayment;
            set
            {
                purchasePayment = value;
                OnPropertyChanged(nameof(PurchasePayment));
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

        private decimal AmountAlreadyPaid;
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

        private UserAccount _selectedPaidBy;
        public UserAccount SelectedPaidBy
        {
            get => _selectedPaidBy;
            set
            {
                _selectedPaidBy = value;
                OnPropertyChanged(nameof(SelectedPaidBy));
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

                    case nameof(SelectedPaidBy):
                        if (SelectedPaidBy == null)
                            return "Paid by is required.";
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

        public AddEditPurchasePaymentViewModel(decimal totalAmount, decimal paidAmount)
        {
            AmountAlreadyPaid = paidAmount;
            TotalBillAmount = totalAmount;
            DueAmount = totalAmount - paidAmount;

            LoadUsers();

            PaymentModes = new ObservableCollection<string>
            {
                "Cash",
                "Card",
                "UPI",
                "Bank Transfer",
                "Cheque"
            };

            SaveCommand = new RelayCommand(SavePayment, obj => true);
            CancelCommand = new RelayCommand(obj => CloseWindow?.Invoke(), obj => true);
        }

        public AddEditPurchasePaymentViewModel(PurchasePayment payment, decimal totalAmount, decimal paidAmount)
            : this(totalAmount, paidAmount)
        {
            PurchasePayment = payment;

            if (PurchasePayment != null)
            {
                PaymentDate = PurchasePayment.PaymentDate;
                PaymentMode = PurchasePayment.PaymentMode;
                AmountPaid = PurchasePayment.AmountPaid;
                DueAmount = PurchasePayment.DueAmount;

                //SelectedPaidBy = Users.FirstOrDefault(u => u.UserID == PurchasePayment.PaidBy);
            }
        }

        private void LoadUsers()
        {
            Users = new ObservableCollection<UserAccount>(
                _context.UserAccounts.OrderBy(u => u.Username).ToList());
        }

        private void RecalculateDue()
        {
            DueAmount = TotalBillAmount - AmountAlreadyPaid - AmountPaid;
        }

        private void SavePayment(object obj)
        {
            isValidationOn = true;

            if (HasErrors)
                return;

            if (PurchasePayment != null)
            {
                PurchasePayment.PaymentDate = PaymentDate;
                PurchasePayment.PaymentMode = PaymentMode;
                PurchasePayment.AmountPaid = AmountPaid;
                PurchasePayment.DueAmount = DueAmount;
                PurchasePayment.PaidBy = SelectedPaidBy?.UserID;
                PurchasePayment.PaidByNavigation = SelectedPaidBy;

                PurchasePayment.ModifiedAt = DateTime.Now;
                PurchasePayment.ModifiedBy = App.LoggedInUser.UserID;
            }
            else
            {
                PurchasePayment = new PurchasePayment()
                {
                    PaymentDate = PaymentDate,
                    PaymentMode = PaymentMode,
                    AmountPaid = AmountPaid,
                    DueAmount = DueAmount,
                    PaidBy = SelectedPaidBy?.UserID,
                    PaidByNavigation = SelectedPaidBy
                };
            }

            CloseWindow?.Invoke();
        }
    }
}
