using BookStore.DAL.Entities;
using BookStore.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public class UserService
    {
        //ko có nhu cầu CRUD User
        //Chỉ có nhu cầu login, authenticate()
        private UserRepo _repo = new();
        public User? Authenticate(string email)
        {
            return _repo.GetOne(email);
        }

        public User? Authenticate(string email, string pass)
        {
            return _repo.GetOne(email, pass);
        }
    }
}
