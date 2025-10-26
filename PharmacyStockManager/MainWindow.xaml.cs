using Microsoft.Extensions.DependencyInjection;
using PharmacyStockManager.Models;
using System.Windows;

namespace PharmacyStockManager.Views
{
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _context;

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

        //private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        //{
        //    MainContent.Content = new DashboardControl();
        //}

        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ProductsControl();
        }

        private void BtnPurchases_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new PurchasesControl(_context);
        }

        private void BtnSales_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SalesControl(_context);
        }

        private void BtnSuppliers_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SuppliersControl(_context);
        }

        private void BtnCustomers_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CustomersControl(_context);
        }

        private void BtnStockLogs_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new StockLogsControl(_context);
        }

        private void BtnExpiryAlerts_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ExpiryAlertsControl(_context);
        }

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CategoriesControl();
        }

        private void BtnPurchaseReturns_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new PurchaseReturnsControl(_context);
        }

        private void BtnSaleReturns_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SaleReturnsControl(_context);
        }

    }
}
