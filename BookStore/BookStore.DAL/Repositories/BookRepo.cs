using BookStore.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Repositories
{
    public class BookRepo
    {
        private BookStoreDbContext _ctx;

        public List<Book> GetALL()
        {
            _ctx = new BookStoreDbContext();
            return _ctx.Books.Include("Category").ToList();
        }

        public void Delete(Book obj)
        {
            _ctx = new();
            _ctx.Books.Remove(obj);
            _ctx.SaveChanges();
        }
        public void Create(Book obj)
        {
            _ctx = new();
            _ctx.Books.Add(obj);
            _ctx.SaveChanges();
        }
        public void Update(Book obj)
        {
            _ctx = new();
            _ctx.Books.Update(obj);
            _ctx.SaveChanges();
        }

        public Book? GetBookById(int bookId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"📚 GetBookById called with BookId: {bookId}");

                // KIỂM TRA _ctx TRƯỚC KHI DÙNG
                if (_ctx == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ _ctx is NULL! Creating new instance...");
                    _ctx = new BookStoreDbContext();
                }

                // KIỂM TRA Books DbSet
                if (_ctx.Books == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ _ctx.Books is NULL!");
                    return null;
                }

                // TRUY VẤN
                var book = _ctx.Books
                    .Include(b => b.Category)
                    .FirstOrDefault(b => b.BookId == bookId);

                if (book != null)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Book found: {book.BookName}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Book not found for BookId: {bookId}");
                }

                return book;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Exception in GetBookById: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return null;
            }
        }
    }
}
