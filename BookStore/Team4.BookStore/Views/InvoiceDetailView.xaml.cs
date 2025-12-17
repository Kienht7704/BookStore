using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Team4.BookStore.Views
{
    /// <summary>
    /// Interaction logic for InvoiceDetailView.xaml
    /// </summary>
    public partial class InvoiceDetailView : UserControl
    {
        private InvoiceDetailService _service = new();
        private List<InvoiceDetail> _allInvoiceDetails = new();

        public InvoiceDetailView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllInvoiceDetails();
        }

        /// <summary>
        /// Load all invoice details from database
        /// </summary>
        private void LoadAllInvoiceDetails()
        {
            try
            {
                _allInvoiceDetails = _service.GetAllInvoiceDetails();
                InvoiceDetailDataGrid.ItemsSource = _allInvoiceDetails;
                UpdateSummary(_allInvoiceDetails);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"L?i khi t?i d? li?u: {ex.Message}", 
                    "L?i", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Update summary statistics
        /// </summary>
        private void UpdateSummary(List<InvoiceDetail> details)
        {
            TotalRecordsLabel.Text = $"{details.Count} b?n ghi";
            
            double totalRevenue = details.Sum(d => d.Subtotal);
            TotalRevenueLabel.Text = $"{totalRevenue:N0} $";
        }

        /// <summary>
        /// Search functionality
        /// </summary>
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTerm = SearchTextBox.Text.Trim();
            
            if (string.IsNullOrEmpty(searchTerm))
            {
                InvoiceDetailDataGrid.ItemsSource = _allInvoiceDetails;
                UpdateSummary(_allInvoiceDetails);
            }
            else
            {
                try
                {
                    var filteredResults = _service.SearchInvoiceDetails(searchTerm);
                    InvoiceDetailDataGrid.ItemsSource = filteredResults;
                    UpdateSummary(filteredResults);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"L?i tìm ki?m: {ex.Message}", 
                        "L?i", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Filter by today
        /// </summary>
        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime today = DateTime.Today;
                var todayDetails = _service.GetInvoiceDetailsByDateRange(today, today);
                InvoiceDetailDataGrid.ItemsSource = todayDetails;
                UpdateSummary(todayDetails);
                
                SearchTextBox.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"L?i khi l?c d? li?u: {ex.Message}", 
                    "L?i", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Filter by this month
        /// </summary>
        private void ThisMonthButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                
                var monthDetails = _service.GetInvoiceDetailsByDateRange(firstDayOfMonth, lastDayOfMonth);
                InvoiceDetailDataGrid.ItemsSource = monthDetails;
                UpdateSummary(monthDetails);
                
                SearchTextBox.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"L?i khi l?c d? li?u: {ex.Message}", 
                    "L?i", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Refresh data
        /// </summary>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            LoadAllInvoiceDetails();
        }

        /// <summary>
        /// Handle selection change
        /// </summary>
        private void InvoiceDetailDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // You can add functionality here if needed when user selects a row
            // For example, show more details or enable/disable buttons
        }
    }
}
