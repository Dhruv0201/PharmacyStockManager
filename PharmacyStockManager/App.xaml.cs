using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PharmacyStockManager.Models;
using PharmacyStockManager.Views;
using System;
using System.IO;
using System.Windows;

namespace PharmacyStockManager
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static IConfiguration Configuration { get; private set; }
        public static UserAccount LoggedInUser { get; private set; }

        public static void SetLoggedInUser(UserAccount user)
        {
            LoggedInUser = user;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Build configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            // Configure DI services
            var services = new ServiceCollection();
            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            // Global exception handler
            this.DispatcherUnhandledException += (sender, args) =>
            {
                MessageBox.Show($"Unexpected error: {args.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };

            //// Show login window via DI
            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register DbContext as scoped (per window)
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            // Register Windows
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
        }
    }
}
