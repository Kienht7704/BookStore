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

        /// <summary>
        /// Get book by ID for POS lookup
        /// </summary>
        public Book? GetById(int bookId)
        {
            _ctx = new();
            return _ctx.Books.Include("Category").FirstOrDefault(b => b.BookId == bookId);
        }

        /// <summary>
        /// Update book stock quantity (decrease after sale)
        /// </summary>
        public void UpdateStock(int bookId, int quantitySold)
        {
            _ctx = new();
            var book = _ctx.Books.Find(bookId);
            if (book != null)
            {
                book.Quantity -= quantitySold;
                if (book.Quantity < 0) book.Quantity = 0;
                _ctx.SaveChanges();
            }
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
    }
}
