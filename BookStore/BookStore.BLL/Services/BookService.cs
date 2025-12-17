using BookStore.DAL.Entities;
using BookStore.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public class BookService
    {
        private BookRepo _repo = new();

        public List<Book> GetALLBook()
        {
            return _repo.GetALL();
        }

        public void DeleteBook(Book obj) => _repo.Delete(obj);

        public void UpdateBook(Book obj) => _repo.Update(obj);

        public void CreateBook(Book obj) => _repo.Create(obj);

        public void CreateBooks(List<Book> books)
        {
            foreach (var book in books)
            {
                _repo.Create(book);
            }
        }

        public Book? GetBookById(int bookId)
        {
            return _repo.GetBookById(bookId);
        }
        public List<Book> SearchByName(string search)
        {
            return _repo.Search(search);
        }

    }
}

