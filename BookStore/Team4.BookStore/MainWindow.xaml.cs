using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Team4.BookStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private BookService _service = new();


        public User X { get; set; }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AirConDataGrid.ItemsSource = _service.GetALLBook(); // include đã nằm trong repo rồi -> ecapsulation
            //màu mè, vẽ vời thêm
            HelloMsgLabel.Content = "Hello, " + X.FullName;

            //phân quyền nút bấm
            if (X.Role == 2)
            {
                CreateButton.IsEnabled = false;
                UpdateButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // bắt được dòng selected là 1 máy lạnh được chọn
            Book? selected = AirConDataGrid.SelectedItem as Book;
            if (selected == null)
            {
                MessageBox.Show("Please select a row before delete.", "Select one", MessageBoxButton.OK, MessageBoxImage.Warning);
            }// không làm phần  confirm are you sure thì bị trừ điểm
            else
            {
                MessageBoxResult answer = MessageBox.Show("Are you sure you want to delete the selected air conditioner?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (answer == MessageBoxResult.No)
                {
                    // không xóa thì thoát hàm
                    return;
                }
                else
                {
                    _service.DeleteBook(selected);
                    AirConDataGrid.ItemsSource = _service.GetALLBook(); // f5 đổ lại lưới vip
                }
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Book? selected = AirConDataGrid.SelectedItem as Book;
            if (selected == null)
            {
                MessageBox.Show("Please select a row before updating.", "Select one", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            } // đoạn sau phải gửi slected sang detail window để sửa
              // bên class DetailWindow khai báo 1 prop x để hứng và show detail dialog lên , không cho nhiều detail xuất hiện
            DetailWindow detail = new();
            detail.X = selected; // nhiều chàng trỏ 1 nàng
            detail.ShowDialog();
            // F5 lại grid để cập nhật info máy lạnh đã sửa
            AirConDataGrid.ItemsSource = _service.GetALLBook();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            // create không cần selected
            DetailWindow detail = new();
            detail.ShowDialog();
            AirConDataGrid.ItemsSource = _service.GetALLBook();
        }
    }
}