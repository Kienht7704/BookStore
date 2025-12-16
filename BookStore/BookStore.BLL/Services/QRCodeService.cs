using QRCoder;
using System;
using System.Drawing;
using System.IO;

namespace BookStore.BLL.Services
{
    /// <summary>
    /// Service for generating QR codes for payment
    /// </summary>
    public class QRCodeService
    {
        /// <summary>
        /// Generate QR code image as byte array
        /// </summary>
        public byte[] GenerateQRCode(int invoiceId, double amount)
        {
            string paymentInfo = $"BookStore Payment\nInvoice: INV-{invoiceId:D6}\nAmount: ${amount:F2}";
            
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(paymentInfo, QRCodeGenerator.ECCLevel.Q);
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    return qrCode.GetGraphic(10); // 10 pixels per module
                }
            }
        }

        /// <summary>
        /// Generate QR code and save to file
        /// </summary>
        public void SaveQRCodeToFile(int invoiceId, double amount, string filePath)
        {
            byte[] qrBytes = GenerateQRCode(invoiceId, amount);
            File.WriteAllBytes(filePath, qrBytes);
        }

        /// <summary>
        /// Generate QR code as Base64 string (for display in WPF)
        /// </summary>
        public string GenerateQRCodeBase64(int invoiceId, double amount)
        {
            byte[] qrBytes = GenerateQRCode(invoiceId, amount);
            return Convert.ToBase64String(qrBytes);
        }
    }
}
