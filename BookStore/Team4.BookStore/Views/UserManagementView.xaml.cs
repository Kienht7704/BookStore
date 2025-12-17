using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System.Windows;
using System.Windows.Controls;

namespace Team4.BookStore.Views
{
    public partial class UserManagementView : UserControl
    {
        private UserService _userService = new();
        public User CurrentUser { get; set; }

        public UserManagementView()
        {
            InitializeComponent();
            this.Loaded += UserManagementView_Loaded;
        }

        private void UserManagementView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUsers();
            ApplyPermissions();
        }
        private void ApplyPermissions()
        {
            if (CurrentUser != null && CurrentUser.RoleId != 1)
            {
                btnAdd.IsEnabled = false;
                btnUpdate.IsEnabled = false;
                btnDelete.IsEnabled = false;
                ResetButton.IsEnabled= false;
            }
        }

        private void LoadUsers()
        {
            dgUsers.ItemsSource = _userService.GetAllUsers();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            UserDetailWindow userDetailWindow = new UserDetailWindow();
            userDetailWindow.ShowDialog();
            LoadUsers();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            User selected = dgUsers.SelectedItem as User;
            if (selected == null)
            {
                MessageBox.Show("Vui l�ng ch?n ng??i d�ng c?n c?p nh?t.", "Ch?n ng??i d�ng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            UserDetailWindow detail = new();
            detail.X = selected;
            detail.ShowDialog();
            LoadUsers();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            User selected = dgUsers.SelectedItem as User;
            if (selected == null)
            {
                MessageBox.Show("Vui l�ng ch?n ng??i d�ng c?n x�a.", "Ch?n ng??i d�ng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show($"B?n c� ch?c mu?n x�a ng??i d�ng '{selected.FullName}'?", "X�c nh?n x�a", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _userService.DeleteUser(selected);
                LoadUsers();
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            dgUsers.ItemsSource = _userService.SearchByName(SearchTextBox.Text);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            LoadUsers();
        }
    }
}
