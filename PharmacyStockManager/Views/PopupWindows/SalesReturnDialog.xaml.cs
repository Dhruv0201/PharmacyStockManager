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

        public SalesReturnDialog(int SaleID)
        {
            InitializeComponent();
            SaleReturnViewModel viewModel = new SaleReturnViewModel(SaleID);
            this.DataContext = viewModel;
            viewModel.CloseWindow += () => this.Close();

        }
    }
}
