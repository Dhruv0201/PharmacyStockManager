using PharmacyStockManager.ViewModel;
using Xceed.Wpf.Toolkit;

namespace PharmacyStockManager.Views.PopupWindows
{
    /// <summary>
    /// Interaction logic for SalesReturnDialog.xaml
    /// </summary>
    public partial class SalesReturnDialog : ChildWindow
    {
        public SalesReturnDialog()
        {
            InitializeComponent();

        }

        public SalesReturnDialog(int SaleId)
        {
            InitializeComponent();
            SaleReturnViewModel viewModel = new SaleReturnViewModel(SaleId);
            this.DataContext = viewModel;
            viewModel.CloseWindow += () => this.Close();

        }
    }
}
