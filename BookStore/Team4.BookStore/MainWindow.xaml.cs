using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System; // Added for DateTime and Exception
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
        private ExcelService _excelService = new();


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
                MessageBoxResult answer = MessageBox.Show("Are you sure you want to delete the selected book?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Error);
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

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
                Title = "Select Excel file to import"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var books = _excelService.ImportBooksFromExcel(openFileDialog.FileName);
                    if (books.Count == 0)
                    {
                        MessageBox.Show("No valid books found in the Excel file.", "Import", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _service.CreateBooks(books);
                    AirConDataGrid.ItemsSource = _service.GetALLBook();
                    MessageBox.Show($"Successfully imported {books.Count} book(s)!", "Import Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing Excel file: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "Save Excel file",
                FileName = $"BookStore_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var books = _service.GetALLBook();
                    _excelService.ExportBooksToExcel(books, saveFileDialog.FileName);
                    MessageBox.Show($"Successfully exported {books.Count} book(s) to Excel!", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting to Excel: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UserManageButton_Click(object sender, RoutedEventArgs e)
        {
            UserDetailWindow userDetail = new();
            userDetail.ShowDialog();
            AirConDataGrid.ItemsSource = _service.GetALLBook();
        }
    }
}