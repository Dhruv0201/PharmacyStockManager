using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using PharmacyStockManager.Views.PopupWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    internal class SuppliersViewModel : ViewModelBase
    {
        private readonly AppDbContext _context = new AppDbContext();

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
                LoadSuppliers(_searchText);
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        public SuppliersViewModel()
        {

            AddCommand = new RelayCommand(_ => AddSupplier());
            EditCommand = new RelayCommand(obj => EditSupplier(obj as Supplier));
            DeleteCommand = new RelayCommand(obj => DeleteSupplier(obj as Supplier));
            RefreshCommand = new RelayCommand(_ => LoadSuppliers());

            LoadSuppliers();
        }

        public void LoadSuppliers(string? filter = null)
        {
            Suppliers.Clear();
            Suppliers = new ObservableCollection<Supplier>(_context.Suppliers.AsNoTracking()
                .Where(s => string.IsNullOrEmpty(filter) || s.SupplierName.Contains(filter))
                .OrderBy(s=>s.SupplierName)
                .ToList());
        }

        private void AddSupplier()
        {
            var dialog = new SupplierDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadSuppliers(SearchText);
            }
        }

        private void EditSupplier(Supplier supplier)
        {

            if (supplier == null) return;
            var dialog = new SupplierDialog(supplier.SupplierId);
            if (dialog.ShowDialog() == true)
            {
                LoadSuppliers(SearchText);
            }
        }

        private void DeleteSupplier(Supplier supplier)
        {
            if (supplier == null) return;

            if (System.Windows.MessageBox.Show(
                $"Delete Supplier '{supplier.SupplierName}'?",
                "Confirm Delete",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.Yes)
            {
                _context.Suppliers.Remove(supplier);
                _context.SaveChanges();
                LoadSuppliers(SearchText);
            }
        }
    }
}
