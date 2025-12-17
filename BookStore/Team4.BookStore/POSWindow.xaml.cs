using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Team4.BookStore
{
    /// <summary>
    /// Interaction logic for POSWindow.xaml
    /// </summary>
    public partial class POSWindow : Window
    {
        private POSService _posService = new();
        private BookService _bookService = new();
        
        // Current staff member (set from MainWindow)
        public User CurrentStaff { get; set; } = null!;

        public POSWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentStaff != null)
            {
                StaffLabel.Text = $"Staff: {CurrentStaff.FullName}";
            }
            RefreshOrderDisplay();
        }

        /// <summary>
        /// Refresh the order display and totals
        /// </summary>
        private void RefreshOrderDisplay()
        {
            OrderDataGrid.ItemsSource = null;
            OrderDataGrid.ItemsSource = _posService.CurrentOrder;

            SubtotalLabel.Text = $"${_posService.CalculateSubtotal():F2}";
            TotalLabel.Text = $"${_posService.CalculateTotal():F2}";

            PayButton.IsEnabled = _posService.CurrentOrder.Count > 0;
        }

        /// <summary>
        /// Handle Enter key in BookId textbox
        /// </summary>
        private void BookIdTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddBookToOrder();
            }
        }

        /// <summary>
        /// Add button click
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddBookToOrder();
        }

        /// <summary>
        /// Add book to order by ID
        /// </summary>
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

        /// <summary>
        /// Search button click - open book list for selection
        /// </summary>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Show a simple dialog with book list
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

        /// <summary>
        /// Remove item from order
        /// </summary>
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

        /// <summary>
        /// Clear entire order
        /// </summary>
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

        /// <summary>
        /// Close window
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_posService.CurrentOrder.Count > 0)
            {
                var result = MessageBox.Show("You have items in your order. Are you sure you want to close?",
                    "Confirm Close", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes) return;
            }

            Close();
        }

        /// <summary>
        /// Process payment
        /// </summary>
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
            paymentWindow.Owner = this;

            if (paymentWindow.ShowDialog() == true)
            {
                // Payment was successful, refresh display
                RefreshOrderDisplay();
                MessageBox.Show("Payment completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
