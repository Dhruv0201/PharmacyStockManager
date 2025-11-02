using PharmacyStockManager.Models;
using PharmacyStockManager.ViewModel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

public class AddEditSupplierViewModel : ViewModelBase, IDataErrorInfo
{
    private readonly AppDbContext _context = new AppDbContext();
    public event Action CloseWindow;

    private string _supplierName = string.Empty;
    public string SupplierName
    {
        get => _supplierName;
        set
        {
            _supplierName = value;
            OnPropertyChanged(nameof(SupplierName));
        }
    }

    private string _mobileNumber = string.Empty;
    public string MobileNumber
    {
        get => _mobileNumber;
        set
        {
            _mobileNumber = value;
            OnPropertyChanged(nameof(MobileNumber));
        }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged(nameof(Email));
        }
    }

    private string _address = string.Empty;
    public string Address
    {
        get => _address;
        set
        {
            _address = value;
            OnPropertyChanged(nameof(Address));
        }
    }

    private Supplier _supplier;
    public Supplier Supplier
    {
        get => _supplier;
        set
        {
            _supplier = value;
            if (_supplier != null)
            {
                SupplierName = _supplier.SupplierName;
                MobileNumber = _supplier.MobileNumber ?? string.Empty;
                Email = _supplier.Email ?? string.Empty;
                Address = _supplier.Address ?? string.Empty;
            }
            OnPropertyChanged(nameof(Supplier));
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
                nameof(SupplierName) when string.IsNullOrEmpty(SupplierName)
                    => "Supplier name is required.",
                nameof(MobileNumber) when string.IsNullOrEmpty(MobileNumber)
                    => "Mobile number is required.",
                nameof(Email) when string.IsNullOrEmpty(Email)
                    => "Email is required.",
                nameof(Address) when string.IsNullOrEmpty(Address)
                    => "Address is required.",
                _ => null
            };
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
                string error = this[prop.Name];
                if (!string.IsNullOrEmpty(error))
                    return true;
            }
            return false;
        }
    }

    public AddEditSupplierViewModel()
    {
        SaveCommand = new RelayCommand(ExecuteSave, (obj) => true);
        CancelCommand = new RelayCommand((obj) => CloseWindow?.Invoke(), (obj) => true);
    }

    public AddEditSupplierViewModel(int supplierId) : this()
    {
        Supplier = _context.Suppliers.Find(supplierId);
    }

    private void ExecuteSave(object obj)
    {
        isValidationOn = true;
        if (HasErrors)
            return;

        if (Supplier == null)
        {
            var newSupplier = new Supplier
            {
                SupplierName = this.SupplierName,
                MobileNumber = this.MobileNumber,
                Email = this.Email,
                Address = this.Address,
                CreatedAt = DateTime.Now
            };
            _context.Suppliers.Add(newSupplier);
        }
        else
        {
            var existingSupplier = _context.Suppliers.Find(Supplier.SupplierId);
            if (existingSupplier != null)
            {
                existingSupplier.SupplierName = this.SupplierName;
                existingSupplier.MobileNumber = this.MobileNumber;
                existingSupplier.Email = this.Email;
                existingSupplier.Address = this.Address;
                existingSupplier.ModifiedAt = DateTime.Now;
            }
        }

        _context.SaveChanges();
        isValidationOn = false;
        CloseWindow?.Invoke();
    }
}
