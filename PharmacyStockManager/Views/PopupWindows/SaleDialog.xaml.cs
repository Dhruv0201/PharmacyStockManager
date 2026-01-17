using PharmacyStockManager.ViewModel;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace PharmacyStockManager.Views.PopupWindows
{
    /// <summary>
    /// Interaction logic for SaleDialog.xaml
    /// </summary>
    public partial class SaleDialog : ChildWindow
    {
        public SaleDialog()
        {
            InitializeComponent();
            AddEditSaleViewModel viewModel = new AddEditSaleViewModel();
            this.DataContext = viewModel;
            viewModel.CloseWindow += () => this.Close();
        }

        public SaleDialog(int SaleID)
        {
            InitializeComponent();
            AddEditSaleViewModel viewModel = new AddEditSaleViewModel(SaleID);
            this.DataContext = viewModel;
            viewModel.CloseWindow += () => this.Close();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(ch => char.IsDigit(ch) || ch == '.');
        }
    }
}
