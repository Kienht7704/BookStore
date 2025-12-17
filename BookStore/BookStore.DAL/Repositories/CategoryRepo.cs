using BookStore.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Repositories
{
    public class CategoryRepo
    {
        private BookStoreDbContext _ctx;

        public List<Category> GetAll()
        {
            _ctx = new();
            return _ctx.Categories.ToList();
        }

        public Category GetById(int id)
        {
            _ctx = new();
            return _ctx.Categories.FirstOrDefault(c => c.CategoryId == id);
        }

        public void Create(Category category)
        {
            _ctx = new();
            _ctx.Categories.Add(category);
            _ctx.SaveChanges();
        }

        public void Update(Category category)
        {
            _ctx = new();
            _ctx.Categories.Update(category);
            _ctx.SaveChanges();
        }

        public void Delete(Category category)
        {
            _ctx = new();
            _ctx.Categories.Remove(category);
            _ctx.SaveChanges();
        }

        public List<Category> SearchByName(string name)
        {
            _ctx = new();
            return _ctx.Categories
                .Where(c => c.BookGenreType.Contains(name))
                .ToList();
        }
    }
}
