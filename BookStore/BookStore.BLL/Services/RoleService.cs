using BookStore.DAL.Entities;
using BookStore.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public class RoleService
    {
        private RoleRepo _repo = new();
        public List<Role> GetAllRoles()
        {
            return _repo.GetAll();
        }
    }
}
