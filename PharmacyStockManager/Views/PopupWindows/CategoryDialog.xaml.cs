using PharmacyStockManager.ViewModel;
using System.Windows;

namespace PharmacyStockManager.Views.PopupWindows
{
    public partial class CategoryDialog : Window
    {
      AddEditCategoryViewModel viewModel;
        public CategoryDialog()
        {
            InitializeComponent();
            viewModel = new AddEditCategoryViewModel(); 
            this.DataContext = viewModel;
            this.viewModel.CloseWindow += ViewModel_CloseWindow;
        }

        public CategoryDialog(int categoryId)
        {
            InitializeComponent();
            viewModel = new AddEditCategoryViewModel(categoryId);
            this.DataContext = viewModel;
            viewModel.CloseWindow += ViewModel_CloseWindow;
        }

        private void ViewModel_CloseWindow()
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
