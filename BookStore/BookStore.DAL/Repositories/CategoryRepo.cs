using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Repositories
{
    public class CategoryRepo
    {
        private BookStoreDbContext _ctx; // không new khi nào sai mới new
        // hàm duy nhất pe không cần crud ncc
        public List<Category> GetAll()
        {
            _ctx = new();
            return _ctx.Categories.ToList();
        } // nhớ chọn đúng túi
    }
}
