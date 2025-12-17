using BookStore.DAL.Entities;
using BookStore.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public class CategoryService
    {
        private CategoryRepo _repo = new();

        public List<Category> GetAllCate()
        {
            return _repo.GetAll();
        }

        public Category GetCategoryById(int id)
        {
            return _repo.GetById(id);
        }

        public void CreateCategory(Category category)
        {
            _repo.Create(category);
        }

        public void UpdateCategory(Category category)
        {
            _repo.Update(category);
        }

        public void DeleteCategory(Category category)
        {
            _repo.Delete(category);
        }

        public List<Category> SearchByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return GetAllCate();
            }
            return _repo.SearchByName(name);
        }
    }
}
