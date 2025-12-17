using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for DetailWindow.xaml
    /// </summary>
    public partial class DetailWindow : Window
    {

        public Book X { get; set; }

        private BookService _bookService = new();
        private CategoryService _categoryService = new();
        public DetailWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            CategoryIdComboBox.ItemsSource = _categoryService.GetAllCate();
            CategoryIdComboBox.DisplayMemberPath = "BookGenreType";
            CategoryIdComboBox.SelectedValuePath = "CategoryId";

            if (X != null)
            {
                DetailTitleLable.Content = "Chỉnh sửa Sách";
                BookIdTextBox.Text = X.BookId.ToString();
                BookIdTextBox.IsEnabled = false;
                BookNameTextBox.Text = X.BookName;
                DescriptionTextBox.Text = X.Description;
                PublicationDateTextBox.Text = X.PublicationDate.ToString("yyyy-MM-dd");
                QuantityTextBox.Text = X.Quantity.ToString();
                PriceTextBox.Text = X.Price.ToString();
                AuthorTextBox.Text = X.Author.ToString();

                CategoryIdComboBox.SelectedValue = X.CategoryId;
                
                // Load ImageUrl nếu có
                if (!string.IsNullOrWhiteSpace(X.ImageUrl))
                {
                    ImageUrlTextBox.Text = X.ImageUrl;
                    LoadImagePreview(X.ImageUrl);
                }
            }
            else
            {
                DetailTitleLable.Content = "Tạo mới Sách";
                BookIdTextBox.Text = _bookService.AutoGenBookId().ToString();
                BookIdTextBox.IsEnabled = false;
            }
        }

        public bool CheckVar()
        {
            string name = BookNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Name is required", "Required", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
            if (name.Length < 5 || name.Length > 100)
            {
                MessageBox.Show("Name must be 5 to 100 character length", "Validation", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckVar() != true)
                {
                    return;
                }
                Book obj = new();
                obj.BookId = int.Parse(BookIdTextBox.Text);
                obj.BookName = BookNameTextBox.Text;
                obj.Description = DescriptionTextBox.Text;
                obj.PublicationDate = DateTime.Parse(PublicationDateTextBox.Text);
                obj.Quantity = int.Parse(QuantityTextBox.Text);
                obj.Price = double.Parse(PriceTextBox.Text);
                obj.Author = AuthorTextBox.Text;
                obj.CategoryId = int.Parse(CategoryIdComboBox.SelectedValue.ToString());
                
                // Lưu ImageUrl
                string imageUrl = ImageUrlTextBox.Text.Trim();
                obj.ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl;

                if (X == null) _bookService.CreateBook(obj);
                else _bookService.UpdateBook(obj);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu sách: {ex.Message}\n\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Xử lý khi nhấn nút Browse để chọn file ảnh
        /// </summary>
        private void BrowseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files (*.*)|*.*",
                Title = "Chọn ảnh bìa sách"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImageUrlTextBox.Text = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Xử lý khi ImageUrl thay đổi để cập nhật preview
        /// </summary>
        private void ImageUrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BookCoverImage == null || ImageUrlTextBox == null) return;
            
            string imageUrl = ImageUrlTextBox.Text?.Trim() ?? "";
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                LoadImagePreview(imageUrl);
            }
            else
            {
                BookCoverImage.Source = null;
            }
        }

        /// <summary>
        /// Load và hiển thị preview hình ảnh từ URL hoặc file path
        /// </summary>
        private void LoadImagePreview(string imageUrl)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                if (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://"))
                {
                    bitmap.UriSource = new Uri(imageUrl);
                }
                else if (File.Exists(imageUrl))
                {
                    bitmap.UriSource = new Uri(imageUrl);
                }
                else
                {
                    BookCoverImage.Source = null;
                    return;
                }

                bitmap.EndInit();
                BookCoverImage.Source = bitmap;
            }
            catch (Exception)
            {
                BookCoverImage.Source = null;
            }
        }
    }
}

