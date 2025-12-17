using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Team4.BookStore.Views
{
    public partial class POSView : UserControl
    {
        private POSService _posService = new();
        private BookService _bookService = new();

        public User CurrentStaff { get; set; }

        public POSView()
        {
            InitializeComponent();
            this.Loaded += POSView_Loaded;
        }

        private void POSView_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentStaff != null)
            {
                StaffLabel.Text = $"Staff: {CurrentStaff.FullName}";
            }
            RefreshOrderDisplay();
        }

        private void RefreshOrderDisplay()
        {
            OrderDataGrid.ItemsSource = null;
            OrderDataGrid.ItemsSource = _posService.CurrentOrder;

            SubtotalLabel.Text = $"{_posService.CalculateSubtotal():N0} VN?";
            TotalLabel.Text = $"{_posService.CalculateTotal():N0} VN?";

            PayButton.IsEnabled = _posService.CurrentOrder.Count > 0;
        }

        private void BookIdTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddBookToOrder();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddBookToOrder();
        }

        private void AddBookToOrder()
        {
            string input = BookIdTextBox.Text.Trim();

            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Please enter a Book ID.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(input, out int bookId))
            {
                MessageBox.Show("Invalid Book ID. Please enter a number.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var (success, message) = _posService.AddToOrder(bookId);

            if (success)
            {
                BookIdTextBox.Clear();
                RefreshOrderDisplay();
            }
            else
            {
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            BookIdTextBox.Focus();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var books = _bookService.GetALLBook();
            string bookList = "Available Books (Enter ID to add):\n\n";

            foreach (var book in books)
            {
                if (book.Quantity > 0)
                {
                    bookList += $"ID: {book.BookId} | {book.BookName} | ${book.Price:F2} | Stock: {book.Quantity}\n";
                }
            }

            MessageBox.Show(bookList, "Book Catalog", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int bookId)
            {
                var (success, message) = _posService.RemoveFromOrder(bookId);
                if (success)
                {
                    RefreshOrderDisplay();
                }
                else
                {
                    MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (_posService.CurrentOrder.Count == 0) return;

            var result = MessageBox.Show("Are you sure you want to clear the order?",
                "Confirm Clear", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _posService.ClearOrder();
                RefreshOrderDisplay();
            }
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (_posService.CurrentOrder.Count == 0)
            {
                MessageBox.Show("No items in order.", "Empty Order", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Open payment window
            PaymentWindow paymentWindow = new PaymentWindow();
            paymentWindow.TotalAmount = _posService.CalculateTotal();
            paymentWindow.POSService = _posService;
            paymentWindow.StaffId = CurrentStaff?.MemberId ?? 1;

            if (paymentWindow.ShowDialog() == true)
            {
                // Payment was successful, refresh display
                RefreshOrderDisplay();
                MessageBox.Show("Payment completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
