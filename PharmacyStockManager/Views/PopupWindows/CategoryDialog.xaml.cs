using System.Windows;

namespace PharmacyStockManager.Views.PopupWindows
{
    public partial class CategoryDialog : Window
    {
        public string CategoryName => txtName.Text.Trim();
        public string Description => txtDesc.Text.Trim();

        public CategoryDialog(string? name = "", string? description = "")
        {
            InitializeComponent();
            txtName.Text = name;
            txtDesc.Text = description;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Category name is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
