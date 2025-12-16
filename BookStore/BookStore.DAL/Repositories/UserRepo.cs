using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Repositories
{
    public class UserRepo
    {
        private BookStoreDbContext? _cxt;

        public User? GetOne(string email)
        {
            _cxt = new();
            return _cxt.Users.FirstOrDefault(x => x.EmailAddress.ToLower() == email.ToLower());

        }

        public User? GetOne(string email, string pass)
        {
            _cxt = new();
            return _cxt.Users.FirstOrDefault(x => x.EmailAddress.ToLower() == email.ToLower() && x.Password == pass);
        }
    }
}