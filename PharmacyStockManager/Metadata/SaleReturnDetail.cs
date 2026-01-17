using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyStockManager.Models
{
    public partial class SaleReturnDetail : INotifyPropertyChanged
    {
        private int soldQuantity;
        private int quantityReturned;
        private decimal unitPrice;
        private decimal? totalPrice;

        [NotMapped]
        public int SoldQuantity
        {
            get => soldQuantity;
            set
            {
                soldQuantity = value;
                OnPropertyChanged(nameof(SoldQuantity));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
