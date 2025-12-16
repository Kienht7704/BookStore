using AForge.Video;
using AForge.Video.DirectShow;
using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using ZXing;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace Team4.BookStore
{
    /// <summary>
    /// Interaction logic for QRScannerWindow.xaml
    /// </summary>
    public partial class QRScannerWindow : Window
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private BarcodeReader barcodeReader;
        private BookService _bookService;
        private bool isScanning = false;
        public QRScannerWindow()
        {
            InitializeComponent();
            barcodeReader = new BarcodeReader
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new ZXing.Common.DecodingOptions
                {
                    PossibleFormats = new List<BarcodeFormat>
        {
            BarcodeFormat.QR_CODE
        }
                }
            };
            _bookService = new BookService();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy danh sách camera
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy camera nào trên máy tính!",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    StatusLabel.Content = "Không tìm thấy camera! ";
                }
                else
                {
                    StatusLabel.Content = $"Tìm thấy {videoDevices.Count} camera.  Sẵn sàng quét! ";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi tạo camera: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (videoDevices == null || videoDevices.Count == 0)
                {
                    MessageBox.Show("Không có camera khả dụng!",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Khởi động camera đầu tiên
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();

                isScanning = true;
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
                StatusLabel.Content = "Đang quét...  Hướng mã QR vào camera";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi động camera:  {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

                Dispatcher.Invoke(() =>
                {
                    CameraImage.Source = BitmapToImageSource(bitmap);
                });

                if (isScanning)
                {
                    // ===== THAY ĐỔI:   QUÉT CẢ QR VÀ BARCODE =====
                    var result = barcodeReader.Decode(bitmap);

                    if (result != null)
                    {
                        string scannedContent = result.Text;
                        string format = result.BarcodeFormat.ToString();

                        Dispatcher.Invoke(() =>
                        {
                            StopScanning();
                            ProcessScannedCode(scannedContent, format);
                        });
                    }
                }

                bitmap.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in NewFrame: {ex.Message}");
            }
        }

        private void ProcessQRCode(string qrContent)
        {
            try
            {
                StatusLabel.Content = "Đã quét được mã QR!  Đang tải thông tin...";

                // Thử parse BookId từ QR code
                if (int.TryParse(qrContent, out int bookId))
                {
                    // Lấy thông tin sách từ database
                    Book? book = _bookService.GetBookById(bookId);

                    if (book != null)
                    {
                        // Hiển thị thông tin sách
                        string message = $"━━━━━━ THÔNG TIN SÁCH ━━━━━━\n\n" +
                                       $"📚 Mã sách: {book.BookId}\n" +
                                       $"📖 Tên sách: {book.BookName}\n" +
                                       $"✍️ Tác giả: {book.Author}\n" +
                                       $"💰 Giá: {book.Price:N0} VNĐ\n" +
                                       $"📦 Số lượng: {book.Quantity}\n" +
                                       $"📅 Ngày xuất bản: {book.PublicationDate:dd/MM/yyyy}\n" +
                                       $"📝 Mô tả:  {book.Description}\n";

                        MessageBox.Show(message, "Thông tin sách",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        StatusLabel.Content = "Quét thành công!  Nhấn 'Bắt đầu quét' để tiếp tục.";
                    }
                    else
                    {
                        MessageBox.Show($"Không tìm thấy sách có mã:  {bookId}",
                            "Không tìm thấy", MessageBoxButton.OK, MessageBoxImage.Warning);
                        StatusLabel.Content = "Không tìm thấy sách!";
                    }
                }
                else
                {
                    MessageBox.Show($"Mã QR không hợp lệ!\nNội dung: {qrContent}",
                        "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Error);
                    StatusLabel.Content = "Mã QR không đúng định dạng!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý mã QR:  {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopScanning();
        }

        private void StopScanning()
        {
            isScanning = false;

            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource.NewFrame -= VideoSource_NewFrame;
            }

            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            StatusLabel.Content = "Đã dừng quét";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Dừng camera khi đóng cửa sổ
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }

        // Helper method:  Chuyển đổi Bitmap sang BitmapImage cho WPF
        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        private void ProcessScannedCode(string content, string format)
        {
            try
            {
                StatusLabel.Content = $"Đã quét được {format}!  Đang tải thông tin...";

                // Trích xuất BookId từ content
                int bookId = ExtractBookId(content);

                if (bookId > 0)
                {
                    Book? book = _bookService.GetBookById(bookId);

                    if (book != null)
                    {
                        string message = $"━━━━━━ THÔNG TIN SÁCH ━━━━━━\n\n" +
                                       $"🔍 Loại mã: {format}\n" +
                                       $"📚 Mã sách: {book.BookId}\n" +
                                       $"📖 Tên sách: {book.BookName}\n" +
                                       $"✍️ Tác giả:  {book.Author}\n" +
                                       $"💰 Giá: {book.Price:N0} VNĐ\n" +
                                       $"📦 Số lượng: {book.Quantity}\n" +
                                       $"📅 Ngày xuất bản: {book.PublicationDate: dd/MM/yyyy}\n" +
                                       $"📝 Mô tả: {book.Description}\n";

                        MessageBox.Show(message, "Thông tin sách",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        StatusLabel.Content = "Quét thành công!  Nhấn 'Bắt đầu quét' để tiếp tục.";
                    }
                    else
                    {
                        MessageBox.Show($"Không tìm thấy sách có mã:  {bookId}",
                            "Không tìm thấy", MessageBoxButton.OK, MessageBoxImage.Warning);
                        StatusLabel.Content = "Không tìm thấy sách! ";
                    }
                }
                else
                {
                    MessageBox.Show($"Mã không hợp lệ!\nNội dung: {content}",
                        "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Error);
                    StatusLabel.Content = "Mã không đúng định dạng!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý mã: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Trích xuất BookId từ QR code hoặc Barcode
        /// </summary>
        private int ExtractBookId(string content)
        {
            try
            {
                // Nếu là QR code thuần túy (chỉ số)
                if (int.TryParse(content, out int directId))
                {
                    return directId;
                }

                // Nếu là Barcode có prefix "BOOK" (VD:  BOOK00000101)
                if (content.StartsWith("BOOK") && content.Length > 4)
                {
                    string numberPart = content.Substring(4); // Bỏ "BOOK"
                    if (int.TryParse(numberPart, out int barcodeId))
                    {
                        return barcodeId;
                    }
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}
