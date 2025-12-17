using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Team4.BookStore.Views
{
    public partial class BookManagementView : UserControl
    {
        private BookService _service = new();
        private ExcelService _excelService = new();
        public User CurrentUser { get; set; }

        public BookManagementView()
        {
            InitializeComponent();
            this.Loaded += BookManagementView_Loaded;
        }

        private void BookManagementView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBooks();
            ApplyPermissions();
        }

        private void LoadBooks()
        {
            BookDataGrid.ItemsSource = _service.GetALLBook();
        }

        private void ApplyPermissions()
        {
            if (CurrentUser != null && CurrentUser.RoleId != 1)
            {
                CreateButton.IsEnabled = false;
                UpdateButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Book? selected = BookDataGrid.SelectedItem as Book;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng ch?n sách tr??c khi xóa.", "Ch?n sách", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult answer = MessageBox.Show("B?n có ch?c ch?n mu?n xóa sách ?ã ch?n?", "Xác nh?n xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (answer == MessageBoxResult.Yes)
            {
                _service.DeleteBook(selected);
                LoadBooks();
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Book? selected = BookDataGrid.SelectedItem as Book;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng ch?n sách tr??c khi c?p nh?t.", "Ch?n sách", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DetailWindow detail = new();
            detail.X = selected;
            detail.ShowDialog();
            LoadBooks();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            DetailWindow detail = new();
            detail.ShowDialog();
            LoadBooks();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
                Title = "Ch?n file Excel ?? import"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var books = _excelService.ImportBooksFromExcel(openFileDialog.FileName);
                    if (books.Count == 0)
                    {
                        MessageBox.Show("Không tìm th?y sách h?p l? trong file Excel.", "Import", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _service.CreateBooks(books);
                    LoadBooks();
                    MessageBox.Show($"Import thành công {books.Count} sách!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"L?i khi import file Excel: {ex.Message}", "L?i Import", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "L?u file Excel",
                FileName = $"BookStore_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var books = _service.GetALLBook();
                    _excelService.ExportBooksToExcel(books, saveFileDialog.FileName);
                    MessageBox.Show($"Export thành công {books.Count} sách ra Excel!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"L?i khi export ra Excel: {ex.Message}", "L?i Export", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void QRScanButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                QRScannerWindow qrWindow = new QRScannerWindow();
                qrWindow.ShowDialog();
                LoadBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"L?i khi m? c?a s? quét QR: {ex.Message}", "L?i", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateQRButton_Click(object sender, RoutedEventArgs e)
        {
            Book? selected = BookDataGrid.SelectedItem as Book;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng ch?n m?t sách ?? t?o mã QR!", "Ch?n sách", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            GenerateQRWindow qrWindow = new GenerateQRWindow();
            qrWindow.SelectedBook = selected;
            qrWindow.ShowDialog();
        }

        private void GenerateBarcodeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Book? selected = BookDataGrid.SelectedItem as Book;
                if (selected == null)
                {
                    MessageBox.Show("Vui lòng ch?n m?t sách ?? t?o Barcode!", "Ch?n sách", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                GenerateBarcodeWindow barcodeWindow = new GenerateBarcodeWindow();
                barcodeWindow.SelectedBook = selected;
                barcodeWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"L?i khi m? c?a s? Barcode: {ex.Message}", "L?i", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            BookDataGrid.ItemsSource = _service.SearchByName(SearchByNameTextBox.Text);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchByNameTextBox.Text = string.Empty;
            LoadBooks();
        }
    }
}
