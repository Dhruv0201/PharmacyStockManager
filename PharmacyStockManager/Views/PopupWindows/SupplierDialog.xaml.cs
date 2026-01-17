using Xceed.Wpf.Toolkit;

namespace PharmacyStockManager.Views.PopupWindows
{
    /// <summary>
    /// Interaction logic for SupplierDialog.xaml
    /// </summary>
    public partial class SupplierDialog : ChildWindow
    {
        AddEditSupplierViewModel viewModel;
        public SupplierDialog()
        {
            InitializeComponent();
            viewModel = new AddEditSupplierViewModel();
            this.DataContext = viewModel;
            this.viewModel.CloseWindow += ViewModel_CloseWindow;
        }

        private void ViewModel_CloseWindow()
        {
            this.DialogResult = true;
            this.Close();
        }

        public SupplierDialog(int SupplierId)
        {
            InitializeComponent();
            viewModel = new AddEditSupplierViewModel(SupplierId);
            this.DataContext = viewModel;
            this.viewModel.CloseWindow += ViewModel_CloseWindow;
        }
    }
}
