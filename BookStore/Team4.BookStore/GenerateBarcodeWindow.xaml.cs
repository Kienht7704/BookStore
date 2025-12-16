using BookStore.DAL.Entities;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace Team4.BookStore
{
    public partial class GenerateBarcodeWindow : Window
    {
        public Book SelectedBook { get; set; }
        private Bitmap barcodeBitmap;
        private byte[] barcodeImageBytes;
        private string barcodeText;

        public GenerateBarcodeWindow()
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
                    MessageBox.Show("❌ Không có thông tin sách! ",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                // HIỂN THỊ THÔNG TIN
                BookInfoText.Text = $"{SelectedBook.BookName}\n(ID: {SelectedBook.BookId})";

                // TẠO BARCODE
                bool success = GenerateBarcode(SelectedBook.BookId.ToString());

                if (!success || barcodeBitmap == null)
                {
                    MessageBox.Show(
                        $"❌ Không thể tạo Barcode!\n\nBookId: {SelectedBook.BookId}",
                        "Lỗi",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Close();
                }
                else
                {
                    MessageBox.Show(
                        $"✅ TẠO BARCODE THÀNH CÔNG!\n\n" +
                        $"Barcode:  {barcodeText}\n" +
                        $"Kích thước: {barcodeBitmap.Width}x{barcodeBitmap.Height}",
                        "Thành công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi:  {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// TẠO BARCODE SỬ DỤNG ZXING
        /// </summary>
        private bool GenerateBarcode(string bookId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"\n========================================");
                System.Diagnostics.Debug.WriteLine($"📊 BẮT ĐẦU TẠO BARCODE");
                System.Diagnostics.Debug.WriteLine($"📝 BookId: {bookId}");
                System.Diagnostics.Debug.WriteLine($"========================================");

                // GIẢI PHÓNG TÀI NGUYÊN CŨ
                if (barcodeBitmap != null)
                {
                    barcodeBitmap.Dispose();
                    barcodeBitmap = null;
                }
                barcodeImageBytes = null;

                // TẠO BARCODE TEXT (Đảm bảo đủ độ dài cho Code128)
                // Thêm prefix "BOOK" để dễ nhận biết
                barcodeText = $"BOOK{bookId.PadLeft(8, '0')}";
                System.Diagnostics.Debug.WriteLine($"📝 Barcode text: {barcodeText}");

                // CẤU HÌNH BARCODE WRITER
                BarcodeWriter barcodeWriter = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128, // Định dạng Code128
                    Options = new EncodingOptions
                    {
                        Width = 500,              // Chiều rộng
                        Height = 150,             // Chiều cao
                        Margin = 10,              // Lề
                        PureBarcode = false       // Hiển thị text bên dưới
                    }
                };

                System.Diagnostics.Debug.WriteLine("✅ BarcodeWriter configured");

                // TẠO BARCODE BITMAP
                barcodeBitmap = barcodeWriter.Write(barcodeText);

                if (barcodeBitmap == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ barcodeBitmap is NULL!");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"✅ Barcode created: {barcodeBitmap.Width}x{barcodeBitmap.Height}");

                // LƯU BACKUP BYTE ARRAY
                using (MemoryStream ms = new MemoryStream())
                {
                    barcodeBitmap.Save(ms, ImageFormat.Png);
                    barcodeImageBytes = ms.ToArray();
                }

                System.Diagnostics.Debug.WriteLine($"✅ Backup saved: {barcodeImageBytes.Length} bytes");

                // HIỂN THỊ LÊN UI
                BitmapImage bitmapImage = BitmapToImageSource(barcodeBitmap);
                if (bitmapImage == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ bitmapImage is NULL!");
                    return false;
                }

                BarcodeImage.Source = bitmapImage;
                BarcodeNumberText.Text = $"Barcode:  {barcodeText}";

                System.Diagnostics.Debug.WriteLine("✅ Barcode hiển thị thành công");
                System.Diagnostics.Debug.WriteLine($"========================================\n");

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ EXCEPTION: {ex.Message}");
                MessageBox.Show($"❌ Lỗi tạo Barcode:\n\n{ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"💾 Save clicked.  Bitmap null: {barcodeBitmap == null}");

                if (barcodeBitmap != null)
                {
                    SaveUsingBitmap();
                }
                else if (barcodeImageBytes != null && barcodeImageBytes.Length > 0)
                {
                    SaveUsingBytes();
                }
                else
                {
                    MessageBox.Show("❌ Barcode chưa được tạo!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi:  {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveUsingBitmap()
        {
            try
            {
                string safeFileName = SanitizeFileName(SelectedBook.BookName);

                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PNG Image|*.png|JPEG Image|*. jpg|Bitmap Image|*.bmp",
                    FileName = $"Barcode_Book_{SelectedBook.BookId}_{safeFileName}",
                    DefaultExt = ".png",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ImageFormat format = GetImageFormat(saveDialog.FileName);
                    barcodeBitmap.Save(saveDialog.FileName, format);

                    MessageBox.Show(
                        $"✅ Lưu Barcode thành công!\n\n📁 {saveDialog.FileName}",
                        "Thành công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    try
                    {
                        System.Diagnostics.Process.Start("explorer. exe",
                            $"/select,\"{saveDialog.FileName}\"");
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi:  {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveUsingBytes()
        {
            try
            {
                string safeFileName = SanitizeFileName(SelectedBook.BookName);

                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PNG Image|*.png",
                    FileName = $"Barcode_Book_{SelectedBook.BookId}_{safeFileName}",
                    DefaultExt = ".png",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };

                if (saveDialog.ShowDialog() == true)
                {
                    File.WriteAllBytes(saveDialog.FileName, barcodeImageBytes);

                    MessageBox.Show(
                        $"✅ Lưu Barcode thành công!\n\n📁 {saveDialog.FileName}",
                        "Thành công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    try
                    {
                        System.Diagnostics.Process.Start("explorer.exe",
                            $"/select,\"{saveDialog.FileName}\"");
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (barcodeBitmap == null && (barcodeImageBytes == null || barcodeImageBytes.Length == 0))
            {
                MessageBox.Show("❌ Barcode chưa được tạo!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                "💡 Để in Barcode:\n\n1. Nhấn 'Yes' để lưu file\n2. Mở file và nhấn Ctrl+P\n\nLưu file ngay? ",
                "Hướng dẫn",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                SaveButton_Click(sender, e);
            }
        }

        private void RegenerateButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateBarcode(SelectedBook.BookId.ToString());
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // ============ HELPER METHODS ============

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