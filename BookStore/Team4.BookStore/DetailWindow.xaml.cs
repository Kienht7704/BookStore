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
                DetailTitleLable.Content = "Chỉnh sửa Sachs";
                // fill thôi 8 cột disable ô key
                BookIdTextBox.Text = X.BookId.ToString();
                BookIdTextBox.IsEnabled = false;
                BookNameTextBox.Text = X.BookName;
                DescriptionTextBox.Text = X.Description;
                QuantityTextBox.Text = X.Quantity.ToString();
                PriceTextBox.Text = X.Price.ToString();
                AuthorTextBox.Text = X.Author.ToString();
                PublicationDataPicker.Text = X.PublicationDate.ToString();

                // phải nhảy đến đúng phần edit mode
                CategoryIdComboBox.SelectedValue = X.CategoryId;
            }
            else
            {
                DetailTitleLable.Content = "Tạo mới Sách";
            }
        }

        public bool CheckVar()
        {
            //check id có rỗng hay không
            //check name có rỗng hay không, có dài từ 5 ... 50 kí tự
            string name = BookNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Name is required", "Required", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
            if (name.Length < 5 || name.Length > 100)
            {
                MessageBox.Show("Name must be 5 to 50 character length", "Validation", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
            //check số quantity có từ 50... 100 hay không, gõ chữ là chửi

            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // phải gọi hàm check var rồi mới chạy code dưới
            if (CheckVar() != true)
            {
                return; //Không thoát check vả không save xuống database
            }
            // chuẩn bị 1 obj máy lạnh mới set các value từ màn hình ô nhập và gửi cho service
            Book obj = new();
            // set từng field 1
            obj.BookId = int.Parse(BookIdTextBox.Text);
            obj.BookName = BookNameTextBox.Text;
            obj.Description = DescriptionTextBox.Text;
            obj.Quantity = int.Parse(QuantityTextBox.Text);
            obj.Price = double.Parse(PriceTextBox.Text);
            obj.Author = AuthorTextBox.Text;
            obj.PublicationDate = PublicationDataPicker.SelectedDate.Value;
            obj.CategoryId = int.Parse(CategoryIdComboBox.SelectedValue.ToString());
            if (X == null) _bookService.CreateBook(obj);
            else _bookService.UpdateBook(obj);
            Close();
        }
    }
}
