using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System;
using System.Windows;

namespace Team4.BookStore
{
    /// <summary>
    /// Interaction logic for PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        public double TotalAmount { get; set; }
        public POSService POSService { get; set; } = null!;
        public int StaffId { get; set; }
        public Invoice? CompletedInvoice { get; private set; }

        public PaymentWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TotalAmountLabel.Text = $"${TotalAmount:F2}";

            // Handle radio button changes for QR display
            CashRadio.Checked += (s, args) => QRCodePanel.Visibility = Visibility.Collapsed;
            QRRadio.Checked += (s, args) => QRCodePanel.Visibility = Visibility.Visible;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string paymentMethod = CashRadio.IsChecked == true ? "Cash" : "QR Code";

            // Process payment
            var (success, message, invoice) = POSService.ProcessPayment(StaffId, paymentMethod);

            if (success && invoice != null)
            {
                CompletedInvoice = invoice;

                // Show receipt window
                ReceiptWindow receiptWindow = new ReceiptWindow();
                receiptWindow.Invoice = invoice;
                receiptWindow.Owner = this.Owner;
                receiptWindow.Show();

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show(message, "Payment Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
