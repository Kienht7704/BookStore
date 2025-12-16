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
    /// Interaction logic for UserDetailWindow.xaml
    /// </summary>
    public partial class UserDetailWindow : Window
    {
        public User X { get; set; }
        private UserService _ctx = new();
        private RoleService _roleService = new();
        public UserDetailWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboRole.ItemsSource = _roleService.GetAllRoles();
            cboRole.DisplayMemberPath = "RoleName";
            cboRole.SelectedValuePath = "RoleId";
            if (X != null)
            {
                txtMemberId.Text = X.MemberId.ToString();
                txtMemberId.IsEnabled = false;
                txtFullName.Text = X.FullName;
                txtEmail.Text = X.EmailAddress;
                txtPassword.Password = X.Password;
                txtEmail.IsEnabled = false;
                cboRole.SelectedValue = X.RoleId;
            }
            else
            {
                txtMemberId.Text = _ctx.AutoGenerateMemberID().ToString();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            User newUser = new User();
            newUser.MemberId = int.Parse(txtMemberId.Text);
            newUser.FullName = txtFullName.Text;
            newUser.EmailAddress = txtEmail.Text;
            newUser.Password = txtPassword.Password;
            newUser.RoleId = (int)cboRole.SelectedValue;
            if (X != null)
            {
                _ctx.UpdateUser(newUser);
                MessageBox.Show("Cập nhật người dùng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                _ctx.AddUser(newUser);
                MessageBox.Show("Thêm người dùng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            Close();
        }
    }
}
