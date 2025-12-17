using BookStore.DAL.Entities;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.IO;

namespace BookStore.BLL.Services
{
    /// <summary>
    /// Service for generating PDF receipts using PdfSharp
    /// </summary>
    public class ReceiptService
    {
        /// <summary>
        /// Generate PDF receipt for an invoice
        /// </summary>
        public void GenerateReceiptPDF(Invoice invoice, string filePath)
        {
            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Receipt - INV-" + invoice.InvoiceId.ToString("D6");

            // Create a page
            PdfPage page = document.AddPage();
            page.Width = XUnit.FromMillimeter(80); // Receipt width (80mm)
            page.Height = XUnit.FromMillimeter(200); // Receipt height

            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Fonts
            XFont titleFont = new XFont("Arial", 14, XFontStyleEx.Bold);
            XFont headerFont = new XFont("Arial", 10, XFontStyleEx.Bold);
            XFont regularFont = new XFont("Arial", 9, XFontStyleEx.Regular);
            XFont smallFont = new XFont("Arial", 7, XFontStyleEx.Regular);

            double y = 20;
            double lineHeight = 14;
            double pageWidth = page.Width.Point;
            double margin = 10;

            // --- Header ---
            gfx.DrawString("BOOKSTORE", titleFont, XBrushes.Black,
                new XRect(0, y, pageWidth, lineHeight), XStringFormats.TopCenter);
            y += lineHeight + 5;

            gfx.DrawString("The Gioi Sach Xu F", regularFont, XBrushes.Black,
                new XRect(0, y, pageWidth, lineHeight), XStringFormats.TopCenter);
            y += lineHeight + 10;

            // --- Invoice Info ---
            gfx.DrawString("Invoice #: INV-" + invoice.InvoiceId.ToString("D6"), regularFont, XBrushes.Black,
                new XRect(margin, y, pageWidth - margin * 2, lineHeight), XStringFormats.TopLeft);
            y += lineHeight;

            gfx.DrawString("Date: " + invoice.InvoiceDate.ToString("yyyy-MM-dd HH:mm"), regularFont, XBrushes.Black,
                new XRect(margin, y, pageWidth - margin * 2, lineHeight), XStringFormats.TopLeft);
            y += lineHeight;

            string staffName = invoice.Staff != null ? invoice.Staff.FullName : "N/A";
            gfx.DrawString("Staff: " + staffName, regularFont, XBrushes.Black,
                new XRect(margin, y, pageWidth - margin * 2, lineHeight), XStringFormats.TopLeft);
            y += lineHeight + 10;

            // --- Separator ---
            gfx.DrawLine(XPens.Black, margin, y, pageWidth - margin, y);
            y += 10;

            // --- Items Header ---
            gfx.DrawString("Item", headerFont, XBrushes.Black, new XRect(margin, y, 100, lineHeight), XStringFormats.TopLeft);
            gfx.DrawString("Qty", headerFont, XBrushes.Black, new XRect(pageWidth - 80, y, 25, lineHeight), XStringFormats.TopLeft);
            gfx.DrawString("Total", headerFont, XBrushes.Black, new XRect(pageWidth - 50, y, 40, lineHeight), XStringFormats.TopLeft);
            y += lineHeight + 5;

            // --- Items ---
            if (invoice.InvoiceDetails != null)
            {
                foreach (var detail in invoice.InvoiceDetails)
                {
                    string bookName = detail.Book != null ? detail.Book.BookName : "Book #" + detail.BookId;
                    if (bookName.Length > 18) bookName = bookName.Substring(0, 15) + "...";

                    gfx.DrawString(bookName, regularFont, XBrushes.Black, 
                        new XRect(margin, y, 100, lineHeight), XStringFormats.TopLeft);
                    gfx.DrawString(detail.Quantity.ToString(), regularFont, XBrushes.Black, 
                        new XRect(pageWidth - 80, y, 25, lineHeight), XStringFormats.TopLeft);
                    gfx.DrawString("$" + detail.Subtotal.ToString("F2"), regularFont, XBrushes.Black, 
                        new XRect(pageWidth - 50, y, 40, lineHeight), XStringFormats.TopLeft);
                    y += lineHeight;
                }
            }
            y += 5;

            // --- Separator ---
            gfx.DrawLine(XPens.Black, margin, y, pageWidth - margin, y);
            y += 10;

            // --- Total ---
            gfx.DrawString("TOTAL: $" + invoice.TotalAmount.ToString("F2"), headerFont, XBrushes.Black,
                new XRect(margin, y, pageWidth - margin * 2, lineHeight), XStringFormats.TopRight);
            y += lineHeight + 5;

            gfx.DrawString("Payment: " + invoice.PaymentMethod, regularFont, XBrushes.Black,
                new XRect(margin, y, pageWidth - margin * 2, lineHeight), XStringFormats.TopRight);
            y += lineHeight + 15;

            // --- Separator ---
            gfx.DrawLine(XPens.Black, margin, y, pageWidth - margin, y);
            y += 10;

            // --- Footer ---
            gfx.DrawString("Thank you for your purchase!", headerFont, XBrushes.Black,
                new XRect(0, y, pageWidth, lineHeight), XStringFormats.TopCenter);
            y += lineHeight;

            gfx.DrawString("Please come again!", regularFont, XBrushes.Black,
                new XRect(0, y, pageWidth, lineHeight), XStringFormats.TopCenter);
            y += lineHeight + 10;

            gfx.DrawString("(c) 2025 Kin - Team 4 BookStore", smallFont, XBrushes.Black,
                new XRect(0, y, pageWidth, lineHeight), XStringFormats.TopCenter);

            // Save the document
            document.Save(filePath);
        }
    }
}
