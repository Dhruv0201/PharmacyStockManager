using PharmacyStockManager.ViewModel;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace PharmacyStockManager.Views.PopupWindows
{
    /// <summary>
    /// Interaction logic for PurchaseDialog.xaml
    /// </summary>
    public partial class PurchaseDialog : ChildWindow
    {
        public PurchaseDialog()
        {
            InitializeComponent();
            AddEditPurchaseViewModel viewModel = new AddEditPurchaseViewModel();
            this.DataContext = viewModel;
            viewModel.CloseWindow += delegate { this.DialogResult = true; this.Close(); };
        }

        public PurchaseDialog(int PurchaseID)
        {
            InitializeComponent();
            AddEditPurchaseViewModel viewModel = new AddEditPurchaseViewModel(PurchaseID);
            this.DataContext = viewModel;
            viewModel.CloseWindow += delegate { this.DialogResult = true; this.Close(); };
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(ch => char.IsDigit(ch) || ch == '.');
        }

        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = e.DataObject.GetData(DataFormats.Text) as string;
                if (!text.All(char.IsDigit))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
