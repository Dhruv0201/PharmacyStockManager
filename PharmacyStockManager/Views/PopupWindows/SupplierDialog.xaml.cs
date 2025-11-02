using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PharmacyStockManager.Views.PopupWindows
{
    /// <summary>
    /// Interaction logic for SupplierDialog.xaml
    /// </summary>
    public partial class SupplierDialog : Window
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
