using BookStore.DAL.Entities;
using Microsoft.EntityFrameworkCore;
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
        public List<User> getAll()
        {
            _cxt = new();
            return _cxt.Users.Include("Role").ToList();
        }
        public void AddNew(User user)
        {
            _cxt = new();
            _cxt.Users.Add(user);
            _cxt.SaveChanges();
        }
        public void Update(User user)
        {
            _cxt = new();
            _cxt.Users.Update(user);
            _cxt.SaveChanges();
        }
        public void Delete(User user)
        {
            _cxt = new();
            _cxt.Users.Remove(user);
            _cxt.SaveChanges();
        }
        public int AutoGenerateMemberID()
        {
            _cxt = new();
            if (_cxt.Users.Count() == 0)
            {
                return 1;
            }
            else
            {
                return _cxt.Users.Max(u => u.MemberId) + 1;
            }
        }
        public List<User> Search(string search)
        {
            _cxt = new();
            return _cxt.Users
                .Include("Role")
                .Where(u => u.FullName.Contains(search) || (u.EmailAddress != null && u.EmailAddress.Contains(search)))
                .ToList();
        }
    }
}