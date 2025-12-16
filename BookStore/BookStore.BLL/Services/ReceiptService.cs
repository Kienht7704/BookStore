using BookStore.DAL.Entities;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using System;
using System.IO;

namespace BookStore.BLL.Services
{
    /// <summary>
    /// Service for generating PDF receipts
    /// </summary>
    public class ReceiptService
    {
        /// <summary>
        /// Generate PDF receipt for an invoice
        /// </summary>
        public void GenerateReceiptPDF(Invoice invoice, string filePath)
        {
            using (PdfWriter writer = new PdfWriter(filePath))
            using (PdfDocument pdf = new PdfDocument(writer))
            using (Document document = new Document(pdf))
            {
                // Set up fonts
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                // Header
                document.Add(new Paragraph("BOOKSTORE - THE GIOI SACH XU F")
                    .SetFont(boldFont)
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph("RECEIPT / INVOICE")
                    .SetFont(boldFont)
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20));

                // Invoice Info
                document.Add(new Paragraph($"Invoice #: INV-{invoice.InvoiceId:D6}")
                    .SetFont(regularFont)
                    .SetFontSize(10));

                document.Add(new Paragraph($"Date: {invoice.InvoiceDate:yyyy-MM-dd HH:mm}")
                    .SetFont(regularFont)
                    .SetFontSize(10));

                document.Add(new Paragraph($"Staff: {invoice.Staff?.FullName ?? "N/A"}")
                    .SetFont(regularFont)
                    .SetFontSize(10)
                    .SetMarginBottom(15));

                // Separator
                document.Add(new Paragraph("─────────────────────────────────────────")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(10));

                // Items Table
                Table table = new Table(new float[] { 4, 1, 2, 2 });
                table.SetWidth(UnitValue.CreatePercentValue(100));

                // Table Header
                table.AddHeaderCell(new Cell().Add(new Paragraph("Item").SetFont(boldFont).SetFontSize(10)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Qty").SetFont(boldFont).SetFontSize(10)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Price").SetFont(boldFont).SetFontSize(10)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Subtotal").SetFont(boldFont).SetFontSize(10)));

                // Table Items
                if (invoice.InvoiceDetails != null)
                {
                    foreach (var detail in invoice.InvoiceDetails)
                    {
                        string bookName = detail.Book?.BookName ?? $"Book #{detail.BookId}";
                        // Truncate long names
                        if (bookName.Length > 30) bookName = bookName.Substring(0, 27) + "...";

                        table.AddCell(new Cell().Add(new Paragraph(bookName).SetFont(regularFont).SetFontSize(9)));
                        table.AddCell(new Cell().Add(new Paragraph(detail.Quantity.ToString()).SetFont(regularFont).SetFontSize(9)));
                        table.AddCell(new Cell().Add(new Paragraph($"${detail.UnitPrice:F2}").SetFont(regularFont).SetFontSize(9)));
                        table.AddCell(new Cell().Add(new Paragraph($"${detail.Subtotal:F2}").SetFont(regularFont).SetFontSize(9)));
                    }
                }

                document.Add(table);
                document.Add(new Paragraph().SetMarginBottom(10));

                // Separator
                document.Add(new Paragraph("─────────────────────────────────────────")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(5));

                // Totals
                document.Add(new Paragraph($"TOTAL: ${invoice.TotalAmount:F2}")
                    .SetFont(boldFont)
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetMarginBottom(10));

                document.Add(new Paragraph($"Payment Method: {invoice.PaymentMethod}")
                    .SetFont(regularFont)
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.RIGHT));

                document.Add(new Paragraph($"Status: {invoice.Status}")
                    .SetFont(regularFont)
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetMarginBottom(20));

                // Separator
                document.Add(new Paragraph("─────────────────────────────────────────")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(10));

                // Footer
                document.Add(new Paragraph("Thank you for your purchase!")
                    .SetFont(boldFont)
                    .SetFontSize(12)
                    .SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph("Please come again!")
                    .SetFont(regularFont)
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(10));

                document.Add(new Paragraph($"© 2025 Kin - Team 4 BookStore")
                    .SetFont(regularFont)
                    .SetFontSize(8)
                    .SetTextAlignment(TextAlignment.CENTER));
            }
        }

        /// <summary>
        /// Generate receipt and return as byte array (for preview or direct printing)
        /// </summary>
        public byte[] GenerateReceiptBytes(Invoice invoice)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (PdfWriter writer = new PdfWriter(ms))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document document = new Document(pdf))
                {
                    // Same content as GenerateReceiptPDF...
                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                    document.Add(new Paragraph("BOOKSTORE - THE GIOI SACH XU F")
                        .SetFont(boldFont)
                        .SetFontSize(18)
                        .SetTextAlignment(TextAlignment.CENTER));

                    document.Add(new Paragraph($"Invoice #: INV-{invoice.InvoiceId:D6}")
                        .SetFont(regularFont)
                        .SetFontSize(10));

                    document.Add(new Paragraph($"Date: {invoice.InvoiceDate:yyyy-MM-dd HH:mm}")
                        .SetFont(regularFont)
                        .SetFontSize(10));

                    document.Add(new Paragraph($"TOTAL: ${invoice.TotalAmount:F2}")
                        .SetFont(boldFont)
                        .SetFontSize(14));
                }
                return ms.ToArray();
            }
        }
    }
}
