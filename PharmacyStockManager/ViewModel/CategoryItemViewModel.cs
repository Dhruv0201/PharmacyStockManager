using PharmacyStockManager.ViewModel;

namespace PharmacyStockManager.ViewModel
{
    public class CategoryItemViewModel : ViewModelBase
    {
        public int SerialNumber { get; set; }
        public int CategoryID { get; set; }

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

        public string CreatedByName { get; set; } = "Unknown";
        public string ModifiedByName { get; set; } = "Unknown";
    }
}
