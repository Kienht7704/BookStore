using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System;
using System.Linq;
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
using Team4.BookStore.Views;

namespace Team4.BookStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BookManagementView _bookManagementView;
        private CategoryManagementView _categoryManagementView;
        private UserManagementView _userManagementView;
        private POSView _posView;
        private InvoiceDetailView _invoiceDetailView;

        public MainWindow()
        {
            InitializeComponent();
        }

        public User X { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Cập nhật tên người dùng - find TextBlock by name
            var textBlock = FindVisualChild<TextBlock>(this, "HelloMsgLabel");
            if (textBlock != null)
            {
                textBlock.Text = X.FullName;
            }

            // Load Book Management View by default
            LoadBookManagementView();
        }

        // Helper method to find child controls
        private static T FindVisualChild<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            if (parent == null) return null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild && child is FrameworkElement fe && fe.Name == name)
                {
                    return typedChild;
                }

                var result = FindVisualChild<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void LoadBookManagementView()
        {
            if (_bookManagementView == null)
            {
                _bookManagementView = new BookManagementView();
                _bookManagementView.CurrentUser = X;
            }
            MainContentControl.Content = _bookManagementView;

            // Update page title
            var titleLabel = FindVisualChild<TextBlock>(this, "PageTitleLabel");
            if (titleLabel != null)
            {
                titleLabel.Text = "Quản lý sách";
            }

            // Highlight active menu
            BookManageButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495e"));
            CategoryManageButton.Background = Brushes.Transparent;
            UserManageButton.Background = Brushes.Transparent;
            POSButton.Background = Brushes.Transparent;
            InvoiceDetailButton.Background = Brushes.Transparent;
        }

        private void LoadCategoryManagementView()
        {
            if (_categoryManagementView == null)
            {
                _categoryManagementView = new CategoryManagementView();
                _categoryManagementView.CurrentUser = X;
            }
            MainContentControl.Content = _categoryManagementView;

            // Update page title
            var titleLabel = FindVisualChild<TextBlock>(this, "PageTitleLabel");
            if (titleLabel != null)
            {
                titleLabel.Text = "Quản lý danh mục";
            }

            // Highlight active menu
            CategoryManageButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495e"));
            BookManageButton.Background = Brushes.Transparent;
            UserManageButton.Background = Brushes.Transparent;
            POSButton.Background = Brushes.Transparent;
            InvoiceDetailButton.Background = Brushes.Transparent;
        }

        private void LoadUserManagementView()
        {
            if (_userManagementView == null)
            {
                _userManagementView = new UserManagementView();
                _userManagementView.CurrentUser = X;
            }
            MainContentControl.Content = _userManagementView;

            // Update page title
            var titleLabel = FindVisualChild<TextBlock>(this, "PageTitleLabel");
            if (titleLabel != null)
            {
                titleLabel.Text = "Quản lý người dùng";
            }

            // Highlight active menu
            UserManageButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495e"));
            BookManageButton.Background = Brushes.Transparent;
            CategoryManageButton.Background = Brushes.Transparent;
            POSButton.Background = Brushes.Transparent;
            InvoiceDetailButton.Background = Brushes.Transparent;
        }

        private void LoadPOSView()
        {
            if (_posView == null)
            {
                _posView = new POSView();
                _posView.CurrentStaff = X;
            }
            MainContentControl.Content = _posView;

            // Update page title
            var titleLabel = FindVisualChild<TextBlock>(this, "PageTitleLabel");
            if (titleLabel != null)
            {
                titleLabel.Text = "Point of Sale";
            }

            // Highlight active menu
            POSButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495e"));
            BookManageButton.Background = Brushes.Transparent;
            CategoryManageButton.Background = Brushes.Transparent;
            UserManageButton.Background = Brushes.Transparent;
            InvoiceDetailButton.Background = Brushes.Transparent;
        }

        private void LoadInvoiceDetailView()
        {
            if (_invoiceDetailView == null)
            {
                _invoiceDetailView = new InvoiceDetailView();
            }
            MainContentControl.Content = _invoiceDetailView;

            // Update page title
            var titleLabel = FindVisualChild<TextBlock>(this, "PageTitleLabel");
            if (titleLabel != null)
            {
                titleLabel.Text = "Xem Hóa Đơn Chi Tiết";
            }

            // Highlight active menu
            InvoiceDetailButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495e"));
            POSButton.Background = Brushes.Transparent;
            BookManageButton.Background = Brushes.Transparent;
            CategoryManageButton.Background = Brushes.Transparent;
            UserManageButton.Background = Brushes.Transparent;
        }

        private void BookManageButton_Click(object sender, RoutedEventArgs e)
        {
            LoadBookManagementView();
        }

        private void CategoryManageButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCategoryManagementView();
        }

        private void UserManageButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserManagementView();
        }

        private void POSButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPOSView();
        }

        private void InvoiceDetailButton_Click(object sender, RoutedEventArgs e)
        {
            LoadInvoiceDetailView();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}