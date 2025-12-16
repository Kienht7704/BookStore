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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private UserService _userService = new();
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // todo làm login sau
            string email = EmailAddressTextBox.Text;
            string pass = PasswordTextBox.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Both email and pass are required!", "Required", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //biến acc hứng record / row trả về nếu match email, pass - tuỳ chiến lược login

            //StaffMember? acc = _userService.Authenticate(email, pass);//chung chung

            User? acc = _userService.Authenticate(email); //chui tung cai

            if (acc == null)
            {
                MessageBox.Show("Email doesn't exist!", " Wrong credentials", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //trường hợp gọi Authen(email, pass) thì code tiếp
            //if role == 1, == 2

            //cách 2: email tồn tại nhưng chưa chắc đúng pass, để chửi chi tiết thêm

            if (acc.Password != pass)
            {
                MessageBox.Show("Invalid password!", " Wrong credentials", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
                //++ thêm số lần sai pass, lock account lại...
            }

            //đoạn này giống cách 1: check role!!!
            if (acc.Role == 3)
            {
                //member không cho vào hệ thống, dù dùng đúng email và pass
                MessageBox.Show("You have no permission to access this app!", "No permission", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //mời vào app
            //login thành công thì main mới được lên
            MainWindow main = new();
            main.X = acc; //X bên Main đã trỏ cùng vào acc bên login rồi
            main.Show();
            Close(); // đóng login window để app có thể tắt đúng cách
        }
    }
}
