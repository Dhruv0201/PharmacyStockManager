using Microsoft.Extensions.DependencyInjection;
using PharmacyStockManager.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PharmacyStockManager.Views
{
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _context;
        private Button _currentSelectedButton;

        public string LoggedInUserName { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(AppDbContext context): this()
        {
            _context = context;

            // Set logged in user's display name
            LoggedInUserName = "Welcome, " + (App.LoggedInUser.FullName ?? App.LoggedInUser.Username);
            DataContext = this;

            // Load default page
            //MainContent.Content = new DashboardControl();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Close MainWindow
            //this.Close();

            // Open LoginWindow again
            var loginWindow = App.ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
            this.Close();

            // Clear logged-in user
            App.SetLoggedInUser(null);
        }

        private void HighlightSelectedButton(Button clickedButton)
        {
            // Reset previous button background
            if (_currentSelectedButton != null)
            {
                _currentSelectedButton.Background = Brushes.Transparent;
                _currentSelectedButton.FontWeight = FontWeights.Normal;
            }

            // Apply highlight to clicked button
            clickedButton.Background = (Brush)new BrushConverter().ConvertFrom("#1B5E20");
            clickedButton.FontWeight = FontWeights.Bold;

            _currentSelectedButton = clickedButton;
        }

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedButton((Button)sender);
            MainContent.Content = new CategoriesControl();
        }

        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedButton((Button)sender);
            MainContent.Content = new ProductsControl();
        }

        private void BtnPurchases_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedButton((Button)sender);
            MainContent.Content = new PurchasesControl(_context);
        }

        private void BtnPurchaseReturns_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedButton((Button)sender);
            MainContent.Content = new PurchaseReturnsControl(_context);
        }

        private void BtnSales_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedButton((Button)sender);
            MainContent.Content = new SalesControl(_context);
        }

        private void BtnSaleReturns_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedButton((Button)sender);
            MainContent.Content = new SaleReturnsControl(_context);
        }

        private void BtnSuppliers_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedButton((Button)sender);
            MainContent.Content = new SuppliersControl(_context);
        }

        private void BtnCustomers_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedButton((Button)sender);
            MainContent.Content = new CustomersControl();
        }

        private void BtnStockLogs_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedButton((Button)sender);
            MainContent.Content = new StockLogsControl(_context);
        }

        private void BtnExpiryAlerts_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedButton((Button)sender);
            MainContent.Content = new ExpiryAlertsControl(_context);
        }
    }
}
