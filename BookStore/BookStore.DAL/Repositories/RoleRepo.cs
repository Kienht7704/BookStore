using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Repositories
{
    public class RoleRepo
    {
        private BookStoreDbContext _ctx;
        public List<Role> GetAll()
        {
            _ctx = new();
            return _ctx.Roles.ToList();
        }
    }
}
