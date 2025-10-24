using Microsoft.EntityFrameworkCore;
using PharmacyStockManager.Helpers;
using PharmacyStockManager.Models;
using PharmacyStockManager.Models;
using PharmacyStockManager.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PharmacyStockManager.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly LoginViewModel loginViewModel;

        public LoginWindow()
        {
            InitializeComponent();
            loginViewModel = new LoginViewModel();
            this.DataContext = loginViewModel;
            loginViewModel.ChangeWindow += LoginViewModel_ChangeWindow;
            loginViewModel.LoginFailed += LoginViewModel_LoginFailed;
        }

        private void LoginViewModel_LoginFailed()
        {
          PasswordBox.Clear();
          txtUserName.Clear();
          txtUserName.Focus();
        }

        private void LoginViewModel_ChangeWindow()
        {
            var mainWindow = new MainWindow(_context);
            mainWindow.Show();
            this.Close();
        }

        private void TogglePasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Visibility == Visibility.Collapsed)
            {
                txtPassword.Text = PasswordBox.Password;
                txtPassword.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Collapsed;
                txtPassword.Focus();
                txtPassword.CaretIndex = txtPassword.Text.Length;
            }
            else
            {
                PasswordBox.Password = txtPassword.Text;
                PasswordBox.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;
                PasswordBox.Focus();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && loginViewModel != null && loginViewModel.LoginCommand.CanExecute(null))
            {
              loginViewModel.LoginCommand.Execute(null);
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((LoginViewModel)this.DataContext).Password = ((PasswordBox)sender).Password; }
        }
    }
}
