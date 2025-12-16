using BookStore.BLL.Services;
using BookStore.DAL.Entities;
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

namespace Team4.BookStore
{
    /// <summary>
    /// Interaction logic for UserManagementWindow.xaml
    /// </summary>
    public partial class UserManagementWindow : Window
    {
        private UserService _user = new();
        public UserManagementWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgUsers.ItemsSource = _user.GetAllUsers();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            UserDetailWindow userDetailWindow = new UserDetailWindow();
            userDetailWindow.ShowDialog();
            dgUsers.ItemsSource = _user.GetAllUsers();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            User selected = dgUsers.SelectedItem as User;
            if (selected == null)
            {
                MessageBox.Show("Please select a user to update.", "No User Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            UserDetailWindow detail = new();
            detail.X = selected;
            detail.ShowDialog();
            dgUsers.ItemsSource = _user.GetAllUsers();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            User selected = dgUsers.SelectedItem as User;
            if (selected == null)
            {
                MessageBox.Show("Please select a user to delete.", "No User Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete user '{selected.FullName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            _user.DeleteUser(selected);
            dgUsers.ItemsSource = _user.GetAllUsers();
        }
    }
}
