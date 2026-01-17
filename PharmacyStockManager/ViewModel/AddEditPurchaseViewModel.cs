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
    public class AddEditPurchaseViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly AppDbContext _context = new AppDbContext();

        public event Action? CloseWindow;

        public ObservableCollection<Supplier> Suppliers { get; set; } = new ObservableCollection<Supplier>();
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        private Purchase _purchase = new Purchase();
        public Purchase Purchase
        {
            get => _purchase;
            set
            {
                _purchase = value;
                OnPropertyChanged(nameof(Purchase));
            }
        }

        private Supplier? _supplier = null;
        public Supplier? Supplier
        {
            get => _supplier;
            set
            {
                _supplier = value;
                Purchase.SupplierId = value?.SupplierId;
                OnPropertyChanged(nameof(Supplier));
            }
        }

        public DateTime? PurchaseDate
        {
            get => Purchase.PurchaseDate;
            set
            {
                Purchase.PurchaseDate = value;
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

        private string _invoiceFileName;
        public string InvoiceFileName
        {
            get => _invoiceFileName;
            set
            {
                _invoiceFileName = value;
                OnPropertyChanged(nameof(InvoiceFileName));
            }
        }


        public decimal FinalTotal
        {
            get => Purchase.TotalAmount;
            set
            {
                Purchase.TotalAmount = value;
                OnPropertyChanged(nameof(FinalTotal));
            }
        }

        private Product? _product = null;
        public Product? Product
        {
            get => _product;
            set
            {
                _product = value;
                OnPropertyChanged(nameof(Product));
            }
        }

        private string? _batchNumber = null;
        public string? BatchNumber
        {
            get => _batchNumber;
            set
            {
                _batchNumber = value;
                OnPropertyChanged(nameof(BatchNumber));
            }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                RecalculateTotalAmount();
            }
        }

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                _unitPrice = value;
                OnPropertyChanged(nameof(UnitPrice));
                RecalculateTotalAmount();
            }
        }

        private decimal _TotalAmount;
        public decimal TotalAmount
        {
            get => _TotalAmount;
            set
            {
                _TotalAmount = value;
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        private PurchasePayment? _selectedPayment = null;
        public PurchasePayment? SelectedPayment
        {
            get => _selectedPayment;
            set
            {
                _selectedPayment = value;
                OnPropertyChanged(nameof(SelectedPayment));
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


        public ObservableCollection<PurchaseDetail> PurchaseDetails { get; set; } = new ObservableCollection<PurchaseDetail>();
        public ObservableCollection<PurchasePayment> PurchasePayments { get; set; } = new ObservableCollection<PurchasePayment>();
        private PurchaseDetail? _editingDetail = null;

        public ICommand AddOrUpdateCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand UploadInvoiceCommand { get; }
        public ICommand OpenInvoiceCommand { get; }
        public ICommand AddPurchasePaymentCommand { get; }
        public ICommand EditPurchasePaymentCommand { get; }
        public ICommand DeletePurchasePaymentCommand { get; }
        private bool _isValidationOn = false;

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if (!_isValidationOn)
                    return null;

                switch (columnName)
                {
                    case nameof(Supplier):
                        if (Supplier == null)
                            return "Supplier is required.";
                        break;

                    case nameof(PurchaseDate):
                        if (PurchaseDate == null)
                            return "Purchase date is required.";
                        break;

                    case nameof(InvoiceNumber):
                        if (string.IsNullOrWhiteSpace(InvoiceNumber))
                            return "Invoice number is required.";
                        break;

                    case nameof(Product):
                        if (Product == null)
                            return "Product is required.";
                        break;

                    case nameof(BatchNumber):
                        if (string.IsNullOrWhiteSpace(BatchNumber))
                            return "Batch number is required.";
                        break;

                    case nameof(Quantity):
                        if (Quantity <= 0)
                            return "Quantity must be greater than zero.";
                        break;

                    case nameof(UnitPrice):
                        if (UnitPrice <= 0)
                            return "Unit price must be greater than zero.";
                        break;

                    case nameof(PurchaseDetails):
                        if (PurchaseDetails == null || PurchaseDetails.Count == 0)
                            return "At least one purchase item is required.";
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



        public AddEditPurchaseViewModel()
        {
            InitLookups();

            Purchase = new Purchase
            {
                PurchaseDate = DateTime.Today
            };

            PurchaseDetails = new ObservableCollection<PurchaseDetail>();
            AddOrUpdateCommand = new RelayCommand(AddOrUpdate);
            EditCommand = new RelayCommand(EditItem);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(_ => CloseWindow?.Invoke());
            UploadInvoiceCommand = new RelayCommand(_ => UploadInvoice());
            OpenInvoiceCommand = new RelayCommand(_ => OpenInvoice(), _ => !string.IsNullOrEmpty(Purchase?.InvoiceImagePath));
            AddPurchasePaymentCommand = new RelayCommand(AddPayment, (obj) => FinalTotal != 0);
            EditPurchasePaymentCommand = new RelayCommand(EditPayment);
            DeletePurchasePaymentCommand = new RelayCommand(DeletePayment);

        }

        public AddEditPurchaseViewModel(int PurchaseId) : this()
        {
            var purchase = _context.Purchases
                .Where(x => x.PurchaseId == PurchaseId).Include(x => x.PurchaseDetails).Include(x=>x.PurchasePayments).ThenInclude(x=>x.PaidByNavigation)
                .FirstOrDefault();

            if (purchase != null)
            {
                Purchase = purchase;
                Supplier = Suppliers.FirstOrDefault(x => x.SupplierId == Purchase.SupplierId);
                PurchaseDate = Purchase.PurchaseDate;

                PurchaseDetails = new ObservableCollection<PurchaseDetail>(
                    Purchase.PurchaseDetails);
                PurchasePayments = new ObservableCollection<PurchasePayment>(purchase.PurchasePayments);

                RecalculateFinalTotal();
                RecalculateDue();
                InvoiceNumber = Purchase.InvoiceNumber;

                if (!string.IsNullOrEmpty(Purchase.InvoiceImagePath))
                {
                    InvoiceFileName = Purchase.InvoiceImagePath;
                }

            }
        }

        private void UploadInvoice()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "All Files|*.*|PDF Files|*.pdf|Images|*.jpg;*.png",
                Multiselect = false
            };

            if (dlg.ShowDialog() == true)
            {
                InvoiceImagePath = dlg.FileName;
                InvoiceFileName = System.IO.Path.GetFileName(dlg.FileName);
            }
        }

        private void OpenInvoice()
        {
            if (string.IsNullOrEmpty(Purchase?.InvoiceImagePath))
                return;

            string appFolder = AppDomain.CurrentDomain.BaseDirectory;
            string invoiceFolder = System.IO.Path.Combine(appFolder, "InvoiceFiles");
            string fullPath = System.IO.Path.Combine(invoiceFolder, Purchase.InvoiceImagePath);

            if (System.IO.File.Exists(fullPath))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = fullPath,
                    UseShellExecute = true
                });
            }
        }


        private void InitLookups()
        {
            Suppliers = new ObservableCollection<Supplier>(
                _context.Suppliers.OrderBy(x => x.SupplierName).ToList());

            Products = new ObservableCollection<Product>(
                _context.Products.OrderBy(x => x.ProductName).ToList());
        }

        private void RecalculateTotalAmount()
        {
            TotalAmount = Quantity * UnitPrice;
        }

        private void AddOrUpdate(object? obj)
        {
            if (Product == null || Quantity <= 0 || UnitPrice <= 0)
                return;

            if (_editingDetail == null)
            {
                PurchaseDetails.Add(new PurchaseDetail
                {
                    Product = Product,
                    ProductId = Product.ProductId,
                    BatchNumber = BatchNumber,
                    Quantity = Quantity,
                    UnitPrice = UnitPrice,
                    TotalPrice = TotalAmount
                });
            }
            else
            {
                var index = PurchaseDetails.IndexOf(_editingDetail);
                if (index >= 0)
                {
                    PurchaseDetails[index] = new PurchaseDetail
                    {
                        PurchaseDetailId = _editingDetail.PurchaseDetailId,
                        Product = Product,
                        ProductId = Product.ProductId,
                        BatchNumber = BatchNumber,
                        Quantity = Quantity,
                        UnitPrice = UnitPrice,
                        TotalPrice = TotalAmount
                    };
                }

                _editingDetail = null;
            }

            ClearEntry();
            RecalculateFinalTotal();
            RecalculateDue();
        }

        private void EditItem(object? obj)
        {
            if (obj is not PurchaseDetail d) return;

            _editingDetail = d;
            Product = d.Product;
            BatchNumber = d.BatchNumber;
            Quantity = d.Quantity;
            UnitPrice = d.UnitPrice;
        }

        private void RecalculateFinalTotal()
        {
            FinalTotal = PurchaseDetails.Sum(x => x.TotalPrice ?? 0);
        }

        private void ClearEntry()
        {
            Product = null;
            BatchNumber = null;
            Quantity = 0;
            UnitPrice = 0;
            TotalAmount = 0;
        }

        private void Save(object? obj)
        {

            Purchase.PurchaseDetails.Clear();
            foreach (var d in PurchaseDetails)
                Purchase.PurchaseDetails.Add(d);
            Purchase.PurchasePayments.Clear();
            foreach (var d in PurchasePayments)
                Purchase.PurchasePayments.Add(d);
            string fileNameOnly = null;

            if (!string.IsNullOrEmpty(InvoiceImagePath) && System.IO.File.Exists(InvoiceImagePath))
            {
                string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                string invoiceFolder = System.IO.Path.Combine(appFolder, "InvoiceFiles");

                if (!System.IO.Directory.Exists(invoiceFolder))
                    System.IO.Directory.CreateDirectory(invoiceFolder);

                string extension = System.IO.Path.GetExtension(InvoiceImagePath);

                string safeInvoice = (InvoiceNumber ?? "Invoice")
                    .Replace(" ", "_")
                    .Replace("/", "_")
                    .Replace("\\", "_")
                    .Replace(":", "_");

                fileNameOnly = $"{safeInvoice}_Invoice{extension}";
                string newFullPath = System.IO.Path.Combine(invoiceFolder, fileNameOnly);

                System.IO.File.Copy(InvoiceImagePath, newFullPath, overwrite: true);
            }

            if (fileNameOnly != null)
            {
                Purchase.InvoiceImagePath = fileNameOnly;
            }

            Purchase.InvoiceNumber = InvoiceNumber;



            Purchase.ModifiedAt = DateTime.Now;
            Purchase.ModifiedBy = App.LoggedInUser.UserId;

            if (Purchase.PurchaseId == 0)
                _context.Purchases.Add(Purchase);
            else
                _context.Purchases.Update(Purchase);



            _context.SaveChanges();
            CloseWindow?.Invoke();
        }

        private void AddPayment(object obj)
        {
            decimal totalAmount = PurchaseDetails.Sum(s => s.TotalPrice ?? 0);
            decimal paidAmount = PurchasePayments.Sum(p => p.AmountPaid);
            var dialog = new PurchasePaymentDialog(totalAmount, paidAmount);
            var main = Application.Current.MainWindow as MainWindow;

            dialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];
            main?.RootLayout.Children.Add(dialog);

            dialog.Closed += delegate
            {
                if (dialog.DialogResult == true)
                {
                    PurchasePayments.Add(dialog.ViewModel.PurchasePayment);
                    RecalculateDue();
                }
                main.RootLayout.Children.Remove(dialog);
            };

            dialog.Show();
        }

        private void EditPayment(object obj)
        {
           if(obj!=null)
                SelectedPayment = obj as PurchasePayment;
            if (SelectedPayment == null)
                return;

            decimal totalAmount = PurchaseDetails.Sum(s => s.TotalPrice ?? 0);
            decimal paidAmount = PurchasePayments.Sum(p => p.AmountPaid) - SelectedPayment.AmountPaid;
            var dialog = new PurchasePaymentDialog(SelectedPayment, totalAmount, paidAmount);
            var main = Application.Current.MainWindow as MainWindow;

            dialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];
            main?.RootLayout.Children.Add(dialog);

            dialog.Closed += delegate
            {
                if (dialog.DialogResult == true)
                {
                    var updated = dialog.ViewModel.PurchasePayment;
                    var index = PurchasePayments.IndexOf(SelectedPayment);
                    PurchasePayments.RemoveAt(index);
                    PurchasePayments.Insert(index, updated);
                }
                main.RootLayout.Children.Remove(dialog);
                RecalculateDue();
            };

            dialog.Show();
        }

        private void DeletePayment(object obj)
        {
            if(obj!=null)
                SelectedPayment = obj as PurchasePayment;
            if (SelectedPayment != null)
            {
                PurchasePayments.Remove(SelectedPayment);
                RecalculateDue();
                SelectedPayment = null;
            }
        }

        private void RecalculateTotal()
        {
            TotalAmount = PurchaseDetails.Sum(d => d.TotalPrice ?? (d.UnitPrice * d.Quantity));
        }

        private void RecalculateDue()
        {
            decimal billAmount = PurchaseDetails?.Sum(s => s?.TotalPrice ?? 0) ?? 0;
            decimal paidAmount = PurchasePayments?.Sum(p => p?.AmountPaid ?? 0) ?? 0;

            TotalPaid = paidAmount;
            DueAmount = billAmount - paidAmount;
        }

    }
}
