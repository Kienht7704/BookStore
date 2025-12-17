using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Team4.BookStore
{
    /// <summary>
    /// Interaction logic for PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        private QRCodeService _qrService = new();
        
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
            QRRadio.Checked += (s, args) => {
                QRCodePanel.Visibility = Visibility.Visible;
                GenerateAndDisplayQRCode();
            };
        }

        /// <summary>
        /// Generate real QR code and display it
        /// </summary>
        private void GenerateAndDisplayQRCode()
        {
            try
            {
                // Generate QR code with order info
                // Using a temporary invoice ID (0) since we haven't created the invoice yet
                byte[] qrBytes = _qrService.GenerateQRCode(0, TotalAmount);
                
                // Convert to BitmapImage for WPF display
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(qrBytes);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                
                // Display in the Image control
                QRCodeImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating QR code: {ex.Message}", "QR Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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
