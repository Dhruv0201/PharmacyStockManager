using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyStockManager.Models
{
    public partial class SaleDetail : INotifyPropertyChanged
    {
        private bool _isSelected;

        [NotMapped]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
