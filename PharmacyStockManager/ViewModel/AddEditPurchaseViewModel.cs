using PharmacyStockManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    internal class AddEditPurchaseViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly AppDbContext _context = new AppDbContext();
        public event Action CloseWindow;

        private ObservableCollection<Supplier> _suppliers;
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set
            {
                _suppliers = value;
                OnPropertyChanged(nameof(Suppliers));
            }
        }

        private ObservableCollection<UserAccount> _users;
        public ObservableCollection<UserAccount> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        private Purchase purchase;
        public Purchase Purchase
        {
            get => purchase;
            set
            {
                purchase = value;
                OnPropertyChanged(nameof(Purchase));
            }
        }

        private Supplier _selectedSupplier;
        public Supplier SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                _selectedSupplier = value;
                OnPropertyChanged(nameof(SelectedSupplier));
            }
        }

        private UserAccount _selectedPurchasedBy;
        public UserAccount SelectedPurchasedBy
        {
            get => _selectedPurchasedBy;
            set
            {
                _selectedPurchasedBy = value;
                OnPropertyChanged(nameof(SelectedPurchasedBy));
            }
        }

        private DateTime? _purchaseDate = DateTime.Now;
        public DateTime? PurchaseDate
        {
            get => _purchaseDate;
            set
            {
                _purchaseDate = value;
                OnPropertyChanged(nameof(PurchaseDate));
            }
        }

        private string _invoiceNumber;
        public string InvoiceNumber
        {
            get => _invoiceNumber;
            set
            {
                _invoiceNumber = value;
                OnPropertyChanged(nameof(InvoiceNumber));
            }
        }

        private string _invoiceImagePath;
        public string InvoiceImagePath
        {
            get => _invoiceImagePath;
            set
            {
                _invoiceImagePath = value;
                OnPropertyChanged(nameof(InvoiceImagePath));
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

        private bool isValidationOn;

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if (!isValidationOn)
                    return null;

                switch (columnName)
                {
                    case nameof(SelectedSupplier):
                        if (SelectedSupplier == null)
                            return "Supplier is required.";
                        break;

                    case nameof(PurchaseDate):
                        if (PurchaseDate == null)
                            return "Purchase Date is required.";
                        break;

                    case nameof(InvoiceNumber):
                        if (string.IsNullOrEmpty(InvoiceNumber))
                            return "Invoice Number is required.";
                        break;

                    case nameof(TotalAmount):
                        if (TotalAmount <= 0)
                            return "Total Amount must be greater than zero.";
                        break;

                    case nameof(SelectedPurchasedBy):
                        if (SelectedPurchasedBy == null)
                            return "Purchased By is required.";
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
                var properties = GetType().GetProperties();
                foreach (var prop in properties)
                {
                    string err = this[prop.Name];
                    if (!string.IsNullOrEmpty(err))
                        return true;
                }
                return false;
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand BrowseInvoiceCommand { get; }

        private void SavePurchase(object obj)
        {
            isValidationOn = true;
            if (HasErrors)
                return;


            string fileNameOnly = null;

            if (!string.IsNullOrEmpty(InvoiceImagePath) && System.IO.File.Exists(InvoiceImagePath))
            {
                string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                string invoiceFolder = System.IO.Path.Combine(appFolder, "InvoiceFiles");

                if (!System.IO.Directory.Exists(invoiceFolder))
                    System.IO.Directory.CreateDirectory(invoiceFolder);

                string extension = System.IO.Path.GetExtension(InvoiceImagePath);

                string safeInvoice = InvoiceNumber
                    .Replace(" ", "_")
                    .Replace("/", "_")
                    .Replace("\\", "_")
                    .Replace(":", "_");

                fileNameOnly = $"{safeInvoice}_Invoice{extension}";
                string newFullPath = System.IO.Path.Combine(invoiceFolder, fileNameOnly);

                System.IO.File.Copy(InvoiceImagePath, newFullPath, overwrite: true);
            }


            if (purchase != null)
            {
                purchase.SupplierId = SelectedSupplier.SupplierId;
                purchase.PurchaseDate = PurchaseDate;
                purchase.InvoiceNumber = InvoiceNumber;

                if (fileNameOnly != null)
                    purchase.InvoiceImagePath = fileNameOnly;

                purchase.TotalAmount = TotalAmount;
                purchase.PurchasedBy = SelectedPurchasedBy?.UserId;
                purchase.ModifiedAt = DateTime.Now;
                purchase.ModifiedBy = App.LoggedInUser.UserId;
            }
            else
            {
                purchase = new Purchase();
                purchase.SupplierId = SelectedSupplier.SupplierId;
                purchase.PurchaseDate = PurchaseDate;
                purchase.InvoiceNumber = InvoiceNumber;
                purchase.InvoiceImagePath = fileNameOnly;
                purchase.TotalAmount = TotalAmount;
                purchase.PurchasedBy = SelectedPurchasedBy?.UserId;

                _context.Purchases.Add(purchase);
            }

            _context.SaveChanges();

            CloseWindow?.Invoke();
        }


        public AddEditPurchaseViewModel()
        {
            BindSuppliersAndUsers();
            SaveCommand = new RelayCommand(SavePurchase, obj => true);
            CancelCommand = new RelayCommand(obj => CloseWindow?.Invoke(), obj => true);
            BrowseInvoiceCommand = new RelayCommand(BrowseInvoice, obj => true);
        }

        public AddEditPurchaseViewModel(int purchaseId) : this()
        {
            purchase = _context.Purchases.Find(purchaseId);

            if (purchase != null)
            {
                SelectedSupplier = Suppliers.FirstOrDefault(s => s.SupplierId == purchase.SupplierId);
                PurchaseDate = purchase.PurchaseDate;
                InvoiceNumber = purchase.InvoiceNumber;
                InvoiceImagePath = purchase.InvoiceImagePath;
                TotalAmount = purchase.TotalAmount;
                SelectedPurchasedBy = Users.FirstOrDefault(u => u.UserId == purchase.PurchasedBy);
            }
        }

        private void BindSuppliersAndUsers()
        {
            Suppliers = new ObservableCollection<Supplier>(_context.Suppliers.OrderBy(s => s.SupplierName).ToList());
            Users = new ObservableCollection<UserAccount>(_context.UserAccounts.OrderBy(u => u.Username).ToList());
        }

        private void BrowseInvoice(object obj)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image, PDF and Excel Files|*.jpg;*.jpeg;*.png;*.bmp;*.pdf;*.xlsx;*.xls";

            if (dlg.ShowDialog() == true)
            {
                InvoiceImagePath = dlg.FileName;
            }
        }
    }
}
