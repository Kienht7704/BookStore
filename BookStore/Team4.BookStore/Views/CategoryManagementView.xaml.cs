using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Team4.BookStore.Views
{
    public partial class CategoryManagementView : UserControl
    {
        private CategoryService _service = new();
        public User CurrentUser { get; set; }

        public CategoryManagementView()
        {
            InitializeComponent();
            this.Loaded += CategoryManagementView_Loaded;
        }

        private void CategoryManagementView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategories();
            ApplyPermissions();
        }

        private void LoadCategories()
        {
            CategoryDataGrid.ItemsSource = _service.GetAllCate();
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
            Category? selected = CategoryDataGrid.SelectedItem as Category;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng ch?n danh m?c tr??c khi xóa.", "Ch?n danh m?c", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult answer = MessageBox.Show("B?n có ch?c ch?n mu?n xóa danh m?c ?ã ch?n?", "Xác nh?n xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (answer == MessageBoxResult.Yes)
            {
                try
                {
                    _service.DeleteCategory(selected);
                    LoadCategories();
                    MessageBox.Show("Xóa danh m?c thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"L?i khi xóa danh m?c: {ex.Message}", "L?i", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Category? selected = CategoryDataGrid.SelectedItem as Category;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng ch?n danh m?c tr??c khi c?p nh?t.", "Ch?n danh m?c", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CategoryDetailWindow detail = new();
            detail.SelectedCategory = selected;
            detail.ShowDialog();
            LoadCategories();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryDetailWindow detail = new();
            detail.ShowDialog();
            LoadCategories();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryDataGrid.ItemsSource = _service.SearchByName(SearchByNameTextBox.Text);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchByNameTextBox.Text = string.Empty;
            LoadCategories();
        }
    }
}
