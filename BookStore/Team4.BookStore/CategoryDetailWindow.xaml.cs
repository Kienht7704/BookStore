using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System;
using System.Windows;

namespace Team4.BookStore
{
    public partial class CategoryDetailWindow : Window
    {
        private CategoryService _service = new();
        public Category SelectedCategory { get; set; }

        public CategoryDetailWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectedCategory != null)
            {
                // Update mode
                DetailTitleLabel.Content = "Update Category";
                CategoryIdTextBox.Text = SelectedCategory.CategoryId.ToString();
                GenreTypeTextBox.Text = SelectedCategory.BookGenreType;
                DescriptionTextBox.Text = SelectedCategory.Description;
                CategoryIdTextBox.IsEnabled = false; 
            }
            else
            {
                // Create mode
                CategoryIdTextBox.Text = _service.AutoGenerateId().ToString();
                CategoryIdTextBox.IsEnabled = false;
                DetailTitleLabel.Content = "Create New Category";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(CategoryIdTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nh?p ID!", "Thi?u thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CategoryIdTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(GenreTypeTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nh?p tên th? lo?i!", "Thi?u thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    GenreTypeTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nh?p mô t?!", "Thi?u thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    DescriptionTextBox.Focus();
                    return;
                }

                if (!int.TryParse(CategoryIdTextBox.Text, out int categoryId))
                {
                    MessageBox.Show("Category ID ph?i là s? nguyên!", "D? li?u không h?p l?", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CategoryIdTextBox.Focus();
                    return;
                }

                if (SelectedCategory == null)
                {
                    // Create new category
                    Category newCategory = new Category
                    {
                        CategoryId = categoryId,
                        BookGenreType = GenreTypeTextBox.Text.Trim(),
                        Description = DescriptionTextBox.Text.Trim()
                    };

                    _service.CreateCategory(newCategory);
                    MessageBox.Show("Thêm danh m?c thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Update existing category
                    SelectedCategory.BookGenreType = GenreTypeTextBox.Text.Trim();
                    SelectedCategory.Description = DescriptionTextBox.Text.Trim();

                    _service.UpdateCategory(SelectedCategory);
                    MessageBox.Show("C?p nh?t danh m?c thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"L?i khi l?u danh m?c: {ex.Message}", "L?i", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
