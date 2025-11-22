using PharmacyStockManager.Models;
using PharmacyStockManager.ViewModel;
using PharmacyStockManager.Views.PopupWindows;
using System.Windows;
using System.Windows.Controls;

namespace PharmacyStockManager.Views
{
    public partial class ProductsControl : UserControl
    {
        public ProductsControl()
        {
            InitializeComponent();
            ProductsViewModel viewmodel = new ProductsViewModel();
            this.DataContext = viewmodel;
            viewmodel.AddPordut += () =>
            {
                MainWindow productsWindow = Window.GetWindow(this) as MainWindow;
                ProductDialog productDialog = new ProductDialog();
                productDialog.Style = (Style)Application.Current.Resources["ChildWindowStyle"];
                productsWindow?.RootLayout.Children.Add(productDialog);
                productDialog.Show();

            };

        }
    }
}
