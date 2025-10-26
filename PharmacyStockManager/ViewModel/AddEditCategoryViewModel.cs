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
    internal class AddEditCategoryViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly AppDbContext _context = new AppDbContext();
        public event Action CloseWindow;
        private string _categoryName = string.Empty;
        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                OnPropertyChanged(nameof(CategoryName)); 
            }
        }
        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                 OnPropertyChanged(nameof(Description)); 
            }
        }

        private Category _category;
        public Category Category
        {
            get => _category;
            set
            {
                _category = value;
                if (_category != null)
                {
                    CategoryName = _category.CategoryName;
                    Description = _category.Description ?? string.Empty;
                }
                OnPropertyChanged(nameof(Category));
            }
        }
        
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }

        bool isValidationOn;

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
        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if(!isValidationOn)
                {
                    return string.Empty;
                }
                switch (columnName)
                {
                    case nameof(CategoryName):
                        if (string.IsNullOrEmpty(CategoryName))
                            return "Category name is required.";
                        break;
                    case nameof(Description):
                        if (string.IsNullOrEmpty(Description))
                            return "Description is required.";
                        break;
                }
                return string.Empty;
            }
        }

        public AddEditCategoryViewModel()
        {
            SaveCommand = new RelayCommand(ExecuteSave, (obj)=>true);
            CancelCommand = new RelayCommand((obj) => CloseWindow?.Invoke(), (obj) => true);
        }

        public AddEditCategoryViewModel(int categoryId) : this()
        {
            Category = _context.Categories.Find(categoryId);
        }

        private void ExecuteSave(object obj)
        {
            isValidationOn = true;
            if (HasErrors)
                return;
            if (Category == null)
            {
                var newCategory = new Category
                {
                    CategoryName = this.CategoryName,
                    Description = this.Description
                };
                _context.Categories.Add(newCategory);
            }
            else
            {
                // Update existing category
                var existingCategory = _context.Categories.Find(Category.CategoryId);
                if (existingCategory != null)
                {
                    existingCategory.CategoryName = this.CategoryName;
                    existingCategory.Description = this.Description;
                }
            }
            _context.SaveChanges();
            isValidationOn = false;
            CloseWindow?.Invoke();
        }

        public AddEditCategoryViewModel(Category category) : this()
        {
            Category = category;
        }
    }
}
