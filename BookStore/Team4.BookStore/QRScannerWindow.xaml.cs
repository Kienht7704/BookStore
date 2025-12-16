using AForge.Video;
using AForge.Video.DirectShow;
using BookStore.BLL.Services;
using BookStore.DAL.Entities;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Windows.Compatibility;

namespace Team4.BookStore
{
    public partial class QRScannerWindow : Window
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private BarcodeReader barcodeReader;
        private BookService _bookService;
        private bool isScanning = false;
        private bool isProcessing = false; 

        public QRScannerWindow()
        {
            InitializeComponent();
            barcodeReader = new BarcodeReader();
            _bookService = new BookService();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy camera nào trên máy tính! ",
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

                // Reset flags
                isScanning = true;
                isProcessing = false;

                // Khởi động camera
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();

                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
                StatusLabel.Content = "Đang quét...  Hướng mã QR/Barcode vào camera";

                System.Diagnostics.Debug.WriteLine("✅ Camera started");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi động camera: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                // Clone frame
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

                // Hiển thị lên UI
                Dispatcher.Invoke(() =>
                {
                    CameraImage.Source = BitmapToImageSource(bitmap);
                });

                // ===== FIX QUAN TRỌNG: KIỂM TRA CẢ 2 CỜ =====
                if (isScanning && !isProcessing)
                {
                    var result = barcodeReader.Decode(bitmap);

                    if (result != null)
                    {
                        // ĐẶT CỜ NGAY ĐỂ TRÁNH XỬ LÝ LẶP
                        isProcessing = true;

                        string scannedContent = result.Text;
                        string format = result.BarcodeFormat.ToString();

                        System.Diagnostics.Debug.WriteLine($"🔍 Detected: {format} - {scannedContent}");

                        // XỬ LÝ TRONG THREAD RIÊNG ĐỂ KHÔNG BLOCK CAMERA
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            StopScanning();
                            ProcessScannedCode(scannedContent, format);
                        }), System.Windows.Threading.DispatcherPriority.Normal);
                    }
                }

                bitmap.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error in NewFrame: {ex.Message}");
            }
        }

        private void ProcessScannedCode(string content, string format)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"\n========================================");
                System.Diagnostics.Debug.WriteLine($"🔄 PROCESSING SCANNED CODE");
                System.Diagnostics.Debug.WriteLine($"Content: {content}");
                System.Diagnostics.Debug.WriteLine($"Format: {format}");
                System.Diagnostics.Debug.WriteLine($"========================================");

                StatusLabel.Content = $"Đã quét được {format}!  Đang tải thông tin...";

                // Trích xuất BookId
                int bookId = ExtractBookId(content);

                if (bookId > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"📝 Extracted BookId: {bookId}");

                    // Truy vấn database
                    Book? book = _bookService.GetBookById(bookId);

                    if (book != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ Book found: {book.BookName}");

                        // Hiển thị thông tin
                        string message = $"━━━━━━ THÔNG TIN SÁCH ━━━━━━\n\n" +
                                       $"🔍 Loại mã: {format}\n" +
                                       $"📚 Mã sách: {book.BookId}\n" +
                                       $"📖 Tên sách:  {book.BookName}\n" +
                                       $"✍️ Tác giả:  {book.Author}\n" +
                                       $"💰 Giá:  {book.Price:N0} VNĐ\n" +
                                       $"📦 Số lượng: {book.Quantity}\n" +
                                       $"📅 Ngày xuất bản: {book.PublicationDate: dd/MM/yyyy}\n";

                        if (!string.IsNullOrEmpty(book.Description))
                        {
                            string shortDesc = book.Description.Length > 100
                                ? book.Description.Substring(0, 100) + "..."
                                : book.Description;
                            message += $"📝 Mô tả:  {shortDesc}\n";
                        }

                        MessageBox.Show(message, "Thông tin sách",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        StatusLabel.Content = "✅ Quét thành công!  Nhấn 'Bắt đầu quét' để tiếp tục.";
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Book not found for BookId: {bookId}");

                        MessageBox.Show($"❌ Không tìm thấy sách có mã:  {bookId}",
                            "Không tìm thấy", MessageBoxButton.OK, MessageBoxImage.Warning);

                        StatusLabel.Content = "❌ Không tìm thấy sách! ";
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Invalid content:  {content}");

                    MessageBox.Show($"❌ Mã không hợp lệ!\n\nNội dung: {content}\n\nVui lòng kiểm tra lại mã QR/Barcode.",
                        "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Error);

                    StatusLabel.Content = "❌ Mã không đúng định dạng!";
                }

                // ===== QUAN TRỌNG: RESET CỜ SAU KHI XỬ LÝ XONG =====
                isProcessing = false;

                System.Diagnostics.Debug.WriteLine($"========================================\n");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Exception in ProcessScannedCode: {ex.Message}");

                MessageBox.Show($"❌ Lỗi khi xử lý mã:\n\n{ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);

                StatusLabel.Content = "❌ Lỗi khi xử lý mã!";

                // Reset cờ
                isProcessing = false;
            }
        }

        private int ExtractBookId(string content)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔍 Extracting BookId from: {content}");

                // Loại bỏ khoảng trắng
                content = content?.Trim();

                if (string.IsNullOrEmpty(content))
                    return 0;

                // Thử parse trực tiếp (QR code đơn giản)
                if (int.TryParse(content, out int directId))
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Direct parse:  {directId}");
                    return directId;
                }

                // Thử parse Barcode có prefix "BOOK"
                if (content.StartsWith("BOOK", StringComparison.OrdinalIgnoreCase))
                {
                    string numberPart = content.Substring(4).TrimStart('0');
                    if (int.TryParse(numberPart, out int barcodeId))
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ Barcode parse: {barcodeId}");
                        return barcodeId;
                    }
                }

                System.Diagnostics.Debug.WriteLine($"❌ Cannot extract BookId from: {content}");
                return 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Exception in ExtractBookId: {ex.Message}");
                return 0;
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopScanning();
        }

        private void StopScanning()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🛑 Stopping camera...");

                isScanning = false;
                isProcessing = false;

                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();

                    // Chờ camera dừng hoàn toàn
                    System.Threading.Thread.Sleep(200);

                    if (videoSource.IsRunning)
                    {
                        videoSource.WaitForStop();
                    }

                    videoSource.NewFrame -= VideoSource_NewFrame;
                    System.Diagnostics.Debug.WriteLine("✅ Camera stopped");
                }

                Dispatcher.Invoke(() =>
                {
                    StartButton.IsEnabled = true;
                    StopButton.IsEnabled = false;
                    StatusLabel.Content = "Đã dừng quét";
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error stopping camera: {ex.Message}");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🚪 Window closing...");

                isScanning = false;
                isProcessing = false;

                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource.WaitForStop();
                    videoSource.NewFrame -= VideoSource_NewFrame;
                }

                System.Diagnostics.Debug.WriteLine("✅ Window closed cleanly");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error in Window_Closing: {ex.Message}");
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            try
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
            catch
            {
                return null;
            }
        }
    }
}