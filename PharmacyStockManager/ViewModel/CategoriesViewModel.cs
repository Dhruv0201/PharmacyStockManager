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
        private readonly AppDbContext _context;

        public ObservableCollection<CategoryItemViewModel> Categories { get; set; } = new();
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

        public CategoriesViewModel(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            AddCommand = new RelayCommand(_ => AddCategory());
            EditCommand = new RelayCommand(obj => EditCategory(obj as CategoryItemViewModel));
            DeleteCommand = new RelayCommand(obj => DeleteCategory(obj as CategoryItemViewModel));
            RefreshCommand = new RelayCommand(_ => LoadCategories());

            LoadCategories();
        }

        public void LoadCategories(string? filter = null)
        {
            Categories.Clear();

            var users = _context.UserAccounts.ToDictionary(u => u.UserId, u => u.Username);

            var categories = _context.Categories
                .Where(c => string.IsNullOrWhiteSpace(filter) || c.CategoryName.Contains(filter))
                .OrderBy(c => c.CategoryId)
                .AsEnumerable()
                .Select((c, index) => new CategoryItemViewModel
                {
                    SerialNumber = index + 1,
                    CategoryID = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    CreatedByName = c.CreatedBy.HasValue && users.TryGetValue(c.CreatedBy.Value, out var created) ? created : "Unknown",
                    ModifiedByName = c.ModifiedBy.HasValue && users.TryGetValue(c.ModifiedBy.Value, out var modified) ? modified : "Unknown"
                })
                .ToList();

            foreach (var c in categories)
                Categories.Add(c);
        }

        private void AddCategory()
        {
            var dialog = new CategoryDialog();
            if (dialog.ShowDialog() == true)
            {
                var newCat = new Category
                {
                    CategoryName = dialog.CategoryName,
                    Description = dialog.Description,
                    CreatedBy = App.LoggedInUser.UserId,
                    ModifiedBy = App.LoggedInUser.UserId,
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now
                };
                _context.Categories.Add(newCat);
                _context.SaveChanges();
                LoadCategories(SearchText);
            }
        }

        private void EditCategory(CategoryItemViewModel? categoryVM)
        {
            if (categoryVM == null) return;

            var category = _context.Categories.Find(categoryVM.CategoryID);
            if (category == null) return;

            var dialog = new CategoryDialog(category.CategoryName, category.Description);
            if (dialog.ShowDialog() == true)
            {
                category.CategoryName = dialog.CategoryName;
                category.Description = dialog.Description;
                category.ModifiedBy = App.LoggedInUser.UserId;
                category.ModifiedAt = DateTime.Now;

                _context.SaveChanges();
                LoadCategories(SearchText);
            }
        }

        private void DeleteCategory(CategoryItemViewModel? categoryVM)
        {
            if (categoryVM == null) return;

            var category = _context.Categories.Find(categoryVM.CategoryID);
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
