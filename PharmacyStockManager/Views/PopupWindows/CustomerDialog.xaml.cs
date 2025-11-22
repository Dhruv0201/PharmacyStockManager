using PharmacyStockManager.ViewModel;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace PharmacyStockManager.Views.PopupWindows
{
    public partial class CustomerDialog : ChildWindow
    {
      AddEditCustomerViewModel viewModel;
        public CustomerDialog()
        {
            InitializeComponent();
            viewModel = new AddEditCustomerViewModel(); 
            this.DataContext = viewModel;
            this.viewModel.CloseWindow += ViewModel_CloseWindow;
        }

        public CustomerDialog(int customerId)
        {
            InitializeComponent();
            viewModel = new AddEditCustomerViewModel(customerId);
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
