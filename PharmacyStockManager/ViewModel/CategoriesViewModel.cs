using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Models;
using PharmacyStockManager.Views.PopupWindows;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    public class CategoriesViewModel : ViewModelBase
    {
        private readonly AppDbContext _context = new AppDbContext();

        private ObservableCollection<Category> _categories = new();
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set{
                _categories = value;
                OnPropertyChanged(nameof(Categories));
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
                LoadCategories(_searchText);
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        public CategoriesViewModel()
        {

            AddCommand = new RelayCommand(_ => AddCategory());
            EditCommand = new RelayCommand(obj => EditCategory(obj as Category));
            DeleteCommand = new RelayCommand(obj => DeleteCategory(obj as Category));
            RefreshCommand = new RelayCommand(_ => LoadCategories());

            LoadCategories();
        }

        public void LoadCategories(string? filter = null)
        {
            Categories.Clear();
            Categories = new ObservableCollection<Category>(_context.Categories.AsNoTracking()
                .Where(c => string.IsNullOrEmpty(filter) || c.CategoryName.Contains(filter))
                .OrderBy(c => c.CategoryName)
                .ToList());
        }

        private void AddCategory()
        {
            var dialog = new CategoryDialog();
            if(dialog.ShowDialog() == true)
            {
                LoadCategories(SearchText);
            }
        }

        private void EditCategory(Category category)
        {

            if (category == null) return;
            var dialog = new CategoryDialog(category.CategoryId);
            if(dialog.ShowDialog() == true)
            {
                LoadCategories(SearchText);
            }
        }

        private void DeleteCategory(Category category)
        {
            if (category == null) return;

            if (System.Windows.MessageBox.Show(
                $"Delete category '{category.CategoryName}'?",
                "Confirm Delete",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.Yes)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                LoadCategories(SearchText);
            }
        }
    }
}
