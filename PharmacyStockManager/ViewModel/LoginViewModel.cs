using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Helpers;
using PharmacyStockManager.Models;
using PharmacyStockManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PharmacyStockManager.ViewModel
{
    class LoginViewModel
    {
        private readonly AppDbContext _context = new AppDbContext();
        public event Action ChangeWindow;
        public event Action LoginFailed;
        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public ICommand LoginCommand { get; }
        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        private bool CanExecuteLogin(object obj)
        {
           return !string.IsNullOrWhiteSpace(Username) && Password != null && Password.Length > 0;
        }
        private void CreateAdminUserIfNotExists()
        {
            try
            {
                var existingAdmin = _context.UserAccounts.AsNoTracking().FirstOrDefault(u => u.Username == "admin");
                if (existingAdmin != null) return;

                string password = "12345";

                // Generate salt
                byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
                string saltBase64 = Convert.ToBase64String(saltBytes);

                // Generate hash
                using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
                string hashBase64 = Convert.ToBase64String(pbkdf2.GetBytes(32));

                var adminUser = new UserAccount
                {
                    Username = "admin",
                    HashedPassword = hashBase64,  // PBKDF2 hash in Base64
                    PasswordHash = saltBase64,    // salt in Base64
                    FullName = "Administrator",
                    Role = "Admin",
                    CreatedAt = DateTime.Now
                };

                _context.UserAccounts.Add(adminUser);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating admin user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteLogin(object obj)
        {
            try
            {
                CreateAdminUserIfNotExists();
                var user = _context.UserAccounts
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Username == username);

                if(user != null)
                {
                    var storedHash = user.HashedPassword?.Trim() ?? "";
                    var storedSalt = user.PasswordHash?.Trim() ?? "";

                    if (PasswordHashHelper.VerifyPassword(password, storedHash, storedSalt))
                    {
                        App.SetLoggedInUser(user);
                        ChangeWindow?.Invoke();
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        LoginFailed.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during login:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
