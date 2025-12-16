using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System;
using System.Windows;

namespace Team4.BookStore
{
    /// <summary>
    /// Interaction logic for ReceiptWindow.xaml
    /// </summary>
    public partial class ReceiptWindow : Window
    {
        private ReceiptService _receiptService = new();
        private InvoiceService _invoiceService = new();

        public Invoice Invoice { get; set; } = null!;

        public ReceiptWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Invoice == null) 
            {
                MessageBox.Show("No invoice data available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            // Load full invoice data with details
            var fullInvoice = _invoiceService.GetInvoiceById(Invoice.InvoiceId);
            if (fullInvoice != null)
            {
                Invoice = fullInvoice;
            }

            // Display invoice info
            InvoiceIdLabel.Text = $"Invoice #: INV-{Invoice.InvoiceId:D6}";
            DateLabel.Text = $"Date: {Invoice.InvoiceDate:yyyy-MM-dd HH:mm}";
            StaffLabel.Text = $"Staff: {Invoice.Staff?.FullName ?? "N/A"}";
            TotalLabel.Text = $"${Invoice.TotalAmount:F2}";
            PaymentMethodLabel.Text = Invoice.PaymentMethod;

            // Display items
            ItemsList.ItemsSource = Invoice.InvoiceDetails;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Save Receipt as PDF",
                FileName = $"Receipt_INV-{Invoice.InvoiceId:D6}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _receiptService.GenerateReceiptPDF(Invoice, saveFileDialog.FileName);
                    MessageBox.Show($"Receipt exported successfully!\n\nSaved to: {saveFileDialog.FileName}", 
                        "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting receipt: {ex.Message}", 
                        "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
