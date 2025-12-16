using BookStore.DAL.Entities;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Team4.BookStore
{
    public partial class GenerateQRWindow : Window
    {
        public Book SelectedBook { get; set; }

        // ===== THAY ĐỔI:  LƯU CẢ BYTE ARRAY ĐỂ AN TOÀN HƠN =====
        private Bitmap qrBitmap;
        private byte[] qrImageBytes; // Backup dữ liệu ảnh

        public GenerateQRWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // KIỂM TRA SÁCH
                if (SelectedBook == null)
                {
                    MessageBox.Show("❌ ERROR: SelectedBook is NULL!\n\nVui lòng chọn sách trước khi mở cửa sổ này.",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                // LOG
                string bookInfo = $"BookId: {SelectedBook.BookId}, BookName: {SelectedBook.BookName}";
                System.Diagnostics.Debug.WriteLine($"📖 Window_Loaded - {bookInfo}");

                // HIỂN THỊ THÔNG TIN
                BookInfoText.Text = $"{SelectedBook.BookName}\n(ID: {SelectedBook.BookId})";

                // TẠO QR CODE
                bool success = GenerateQRCode(SelectedBook.BookId.ToString());

                if (!success || qrBitmap == null || qrImageBytes == null)
                {
                    MessageBox.Show(
                        $"❌ KHÔNG THỂ TẠO MÃ QR!\n\n" +
                        $"BookId: {SelectedBook.BookId}\n" +
                        $"qrBitmap:  {(qrBitmap == null ? "NULL" : "OK")}\n" +
                        $"qrImageBytes: {(qrImageBytes == null ? "NULL" : $"{qrImageBytes.Length} bytes")}\n\n" +
                        $"Vui lòng kiểm tra:\n" +
                        $"1. Thư viện QRCoder đã cài chưa?\n" +
                        $"2. BookId có hợp lệ không?\n" +
                        $"3. Xem Output window để biết chi tiết",
                        "Lỗi Tạo QR",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Close();
                }
                else
                {
                    MessageBox.Show(
                        $"✅ TẠO QR THÀNH CÔNG!\n\n" +
                        $"qrBitmap: {qrBitmap.Width}x{qrBitmap.Height}\n" +
                        $"qrImageBytes: {qrImageBytes.Length} bytes\n\n" +
                        $"Bạn có thể lưu hoặc in ngay bây giờ! ",
                        "Thành công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ EXCEPTION trong Window_Loaded:\n\n{ex.Message}\n\n{ex.StackTrace}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// TẠO QR CODE - PHIÊN BẢN CẢI TIẾN
        /// </summary>
        private bool GenerateQRCode(string bookId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"\n========================================");
                System.Diagnostics.Debug.WriteLine($"🔄 BẮT ĐẦU TẠO QR CODE");
                System.Diagnostics.Debug.WriteLine($"📝 BookId: {bookId}");
                System.Diagnostics.Debug.WriteLine($"========================================");

                // GIẢI PHÓNG TÀI NGUYÊN CŨ
                if (qrBitmap != null)
                {
                    qrBitmap.Dispose();
                    qrBitmap = null;
                    System.Diagnostics.Debug.WriteLine("🗑️ Đã dispose qrBitmap cũ");
                }

                qrImageBytes = null;

                // BƯỚC 1: TẠO QR GENERATOR
                System.Diagnostics.Debug.WriteLine("1️⃣ Tạo QRCodeGenerator.. .");
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                if (qrGenerator == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ qrGenerator is NULL!");
                    return false;
                }
                System.Diagnostics.Debug.WriteLine("✅ qrGenerator OK");

                // BƯỚC 2: TẠO QR DATA
                System.Diagnostics.Debug.WriteLine("2️⃣ Tạo QRCodeData...");
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(bookId, QRCodeGenerator.ECCLevel.Q);
                if (qrCodeData == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ qrCodeData is NULL!");
                    return false;
                }
                System.Diagnostics.Debug.WriteLine("✅ qrCodeData OK");

                // BƯỚC 3: TẠO QR CODE OBJECT
                System.Diagnostics.Debug.WriteLine("3️⃣ Tạo QRCode object...");
                QRCode qrCode = new QRCode(qrCodeData);
                if (qrCode == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ qrCode is NULL!");
                    return false;
                }
                System.Diagnostics.Debug.WriteLine("✅ qrCode OK");

                // BƯỚC 4: TẠO BITMAP
                System.Diagnostics.Debug.WriteLine("4️⃣ Gọi GetGraphic()...");
                qrBitmap = qrCode.GetGraphic(
                    pixelsPerModule: 20,
                    darkColor: Color.Black,
                    lightColor: Color.White,
                    drawQuietZones: true
                );

                if (qrBitmap == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ qrBitmap is NULL after GetGraphic()!");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"✅ qrBitmap created: {qrBitmap.Width}x{qrBitmap.Height} pixels");

                // BƯỚC 5: LƯU BACKUP DỮ LIỆU ẢNH
                System.Diagnostics.Debug.WriteLine("5️⃣ Lưu backup byte array...");
                using (MemoryStream ms = new MemoryStream())
                {
                    qrBitmap.Save(ms, ImageFormat.Png);
                    qrImageBytes = ms.ToArray();
                }

                if (qrImageBytes == null || qrImageBytes.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine("❌ qrImageBytes is NULL or empty!");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"✅ qrImageBytes saved: {qrImageBytes.Length} bytes");

                // BƯỚC 6: HIỂN THỊ LÊN GIAO DIỆN
                System.Diagnostics.Debug.WriteLine("6️⃣ Hiển thị lên UI...");
                BitmapImage bitmapImage = BitmapToImageSource(qrBitmap);

                if (bitmapImage == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ bitmapImage is NULL!");
                    return false;
                }

                QRImage.Source = bitmapImage;
                System.Diagnostics.Debug.WriteLine("✅ QR hiển thị trên UI thành công");

                // KIỂM TRA CUỐI CÙNG
                System.Diagnostics.Debug.WriteLine($"\n🎉 KẾT QUẢ CUỐI:");
                System.Diagnostics.Debug.WriteLine($"   qrBitmap: {(qrBitmap != null ? "✅ NOT NULL" : "❌ NULL")}");
                System.Diagnostics.Debug.WriteLine($"   qrImageBytes: {(qrImageBytes != null ? $"✅ {qrImageBytes.Length} bytes" : "❌ NULL")}");
                System.Diagnostics.Debug.WriteLine($"========================================\n");

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"\n❌❌❌ EXCEPTION trong GenerateQRCode ❌❌❌");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"========================================\n");

                MessageBox.Show(
                    $"❌ Exception trong GenerateQRCode:\n\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                qrBitmap = null;
                qrImageBytes = null;
                return false;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"\n========================================");
                System.Diagnostics.Debug.WriteLine($"💾 SAVE BUTTON CLICKED");
                System.Diagnostics.Debug.WriteLine($"   qrBitmap is null: {qrBitmap == null}");
                System.Diagnostics.Debug.WriteLine($"   qrImageBytes is null: {qrImageBytes == null}");
                System.Diagnostics.Debug.WriteLine($"========================================");

                // PHƯƠNG ÁN 1: DÙNG qrBitmap
                if (qrBitmap != null)
                {
                    System.Diagnostics.Debug.WriteLine("✅ Sử dụng qrBitmap để lưu");
                    SaveUsingBitmap();
                    return;
                }

                // PHƯƠNG ÁN 2: DÙNG qrImageBytes (backup)
                if (qrImageBytes != null && qrImageBytes.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ qrBitmap null, dùng qrImageBytes backup");
                    SaveUsingBytes();
                    return;
                }

                // PHƯƠNG ÁN 3: TẠO LẠI QR
                System.Diagnostics.Debug.WriteLine("⚠️ Cả 2 đều null, thử tạo lại QR.. .");
                MessageBoxResult result = MessageBox.Show(
                    "❌ Mã QR bị mất!\n\nBạn muốn tạo lại mã QR không?",
                    "Lỗi",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = GenerateQRCode(SelectedBook.BookId.ToString());
                    if (success && qrBitmap != null)
                    {
                        SaveUsingBitmap();
                    }
                    else
                    {
                        MessageBox.Show("❌ Không thể tạo lại mã QR!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// LƯU BẰNG BITMAP
        /// </summary>
        private void SaveUsingBitmap()
        {
            try
            {
                string safeFileName = SanitizeFileName(SelectedBook.BookName);

                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PNG Image|*.png|JPEG Image|*. jpg|Bitmap Image|*.bmp",
                    FileName = $"QR_Book_{SelectedBook.BookId}_{safeFileName}",
                    DefaultExt = ".png",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ImageFormat format = GetImageFormat(saveDialog.FileName);
                    qrBitmap.Save(saveDialog.FileName, format);

                    System.Diagnostics.Debug.WriteLine($"✅ File saved: {saveDialog.FileName}");

                    MessageBox.Show(
                        $"✅ Lưu mã QR thành công!\n\n📁 {saveDialog.FileName}",
                        "Thành công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    try
                    {
                        System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{saveDialog.FileName}\"");
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi SaveUsingBitmap: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// LƯU BẰNG BYTE ARRAY (BACKUP)
        /// </summary>
        private void SaveUsingBytes()
        {
            try
            {
                string safeFileName = SanitizeFileName(SelectedBook.BookName);

                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PNG Image|*.png",
                    FileName = $"QR_Book_{SelectedBook.BookId}_{safeFileName}",
                    DefaultExt = ".png",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };

                if (saveDialog.ShowDialog() == true)
                {
                    File.WriteAllBytes(saveDialog.FileName, qrImageBytes);

                    System.Diagnostics.Debug.WriteLine($"✅ File saved from bytes: {saveDialog.FileName}");

                    MessageBox.Show(
                        $"✅ Lưu mã QR thành công (từ backup)!\n\n📁 {saveDialog.FileName}",
                        "Thành công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    try
                    {
                        System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{saveDialog.FileName}\"");
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi SaveUsingBytes: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"🖨️ PrintButton clicked.  qrBitmap null: {qrBitmap == null}, qrImageBytes null: {qrImageBytes == null}");

            if (qrBitmap == null && (qrImageBytes == null || qrImageBytes.Length == 0))
            {
                MessageBox.Show("❌ Mã QR chưa được tạo!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                "💡 Để in mã QR:\n\n1. Nhấn 'Yes' để lưu file\n2. Mở file và nhấn Ctrl+P\n\nLưu file ngay? ",
                "Hướng dẫn",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                SaveButton_Click(sender, e);
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            if (bitmap == null) return null;

            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, ImageFormat.Png);
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ BitmapToImageSource error: {ex.Message}");
                return null;
            }
        }

        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return "Unknown";
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            if (fileName.Length > 50) fileName = fileName.Substring(0, 50);
            return fileName;
        }

        private ImageFormat GetImageFormat(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            return extension switch
            {
                ". png" => ImageFormat.Png,
                ".jpg" or ".jpeg" => ImageFormat.Jpeg,
                ".bmp" => ImageFormat.Bmp,
                _ => ImageFormat.Png
            };
        }
    }
}