using BookStore.DAL.Entities;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public class ExcelService
    {
        /// <summary>
        /// Import books from Excel file
        /// </summary>
        public List<Book> ImportBooksFromExcel(string filePath)
        {
            var books = new List<Book>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed();

                bool isFirstRow = true;
                foreach (var row in rows)
                {
                    // Skip header row
                    if (isFirstRow)
                    {
                        isFirstRow = false;
                        continue;
                    }

                    try
                    {
                        var book = new Book
                        {
                            BookId = row.Cell(1).GetValue<int>(),
                            BookName = row.Cell(2).GetString(),
                            Description = row.Cell(3).GetString(),
                            PublicationDate = row.Cell(4).GetDateTime(),
                            Quantity = row.Cell(5).GetValue<int>(),
                            Price = row.Cell(6).GetValue<double>(),
                            Author = row.Cell(7).GetString(),
                            CategoryId = row.Cell(8).GetValue<int>(),
                            ImageUrl = row.Cell(9).GetString()
                        };

                        // Handle empty ImageUrl
                        if (string.IsNullOrWhiteSpace(book.ImageUrl))
                        {
                            book.ImageUrl = null;
                        }

                        books.Add(book);
                    }
                    catch (Exception)
                    {
                        // Skip invalid rows
                        continue;
                    }
                }
            }

            return books;
        }

        /// <summary>
        /// Export books to Excel file
        /// </summary>
        public void ExportBooksToExcel(List<Book> books, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Books");

                // Set font Unicode hỗ trợ tiếng Việt cho toàn bộ worksheet
                worksheet.Style.Font.FontName = "Arial";
                worksheet.Style.Font.FontSize = 11;

                // Header row
                worksheet.Cell(1, 1).Value = "BookId";
                worksheet.Cell(1, 2).Value = "BookName";
                worksheet.Cell(1, 3).Value = "Description";
                worksheet.Cell(1, 4).Value = "PublicationDate";
                worksheet.Cell(1, 5).Value = "Quantity";
                worksheet.Cell(1, 6).Value = "Price";
                worksheet.Cell(1, 7).Value = "Author";
                worksheet.Cell(1, 8).Value = "CategoryId";
                worksheet.Cell(1, 9).Value = "CategoryName";
                worksheet.Cell(1, 10).Value = "ImageUrl";

                // Style header
                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data rows
                int row = 2;
                foreach (var book in books)
                {
                    worksheet.Cell(row, 1).Value = book.BookId;
                    worksheet.Cell(row, 2).Value = book.BookName;
                    worksheet.Cell(row, 3).Value = book.Description;
                    worksheet.Cell(row, 4).Value = book.PublicationDate;
                    worksheet.Cell(row, 5).Value = book.Quantity;
                    worksheet.Cell(row, 6).Value = book.Price;
                    worksheet.Cell(row, 7).Value = book.Author;
                    worksheet.Cell(row, 8).Value = book.CategoryId;
                    worksheet.Cell(row, 9).Value = book.Category?.BookGenreType ?? "";
                    worksheet.Cell(row, 10).Value = book.ImageUrl ?? "";
                    row++;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(filePath);
            }
        }

        /// <summary>
        /// Create a sample template Excel file
        /// </summary>
        public void CreateTemplateExcel(string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Books");

                // Header row
                worksheet.Cell(1, 1).Value = "BookId";
                worksheet.Cell(1, 2).Value = "BookName";
                worksheet.Cell(1, 3).Value = "Description";
                worksheet.Cell(1, 4).Value = "PublicationDate";
                worksheet.Cell(1, 5).Value = "Quantity";
                worksheet.Cell(1, 6).Value = "Price";
                worksheet.Cell(1, 7).Value = "Author";
                worksheet.Cell(1, 8).Value = "CategoryId";
                worksheet.Cell(1, 9).Value = "ImageUrl";

                // Style header
                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightGreen;

                // Sample row
                worksheet.Cell(2, 1).Value = 100;
                worksheet.Cell(2, 2).Value = "Sample Book Name";
                worksheet.Cell(2, 3).Value = "Book description here";
                worksheet.Cell(2, 4).Value = DateTime.Now;
                worksheet.Cell(2, 5).Value = 10;
                worksheet.Cell(2, 6).Value = 19.99;
                worksheet.Cell(2, 7).Value = "Author Name";
                worksheet.Cell(2, 8).Value = 1;
                worksheet.Cell(2, 9).Value = "images/sample.jpg";

                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(filePath);
            }
        }
    }
}
