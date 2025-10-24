using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Helpers;
using PharmacyStockManager.Models;
using PharmacyStockManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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

        private void ExecuteLogin(object obj)
        {
            try
            {
                var user = _context.UserAccounts
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Username == username);
                // user.PasswordHash and user.PasswordSalt are the saved Base64 strings
                if (user != null && PasswordHashHelper.VerifyPassword(password, user.HashedPassword, user.PasswordSalt))
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
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during login:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
