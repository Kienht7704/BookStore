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
        private UserRepo _repo = new();
        public User? Authenticate(string email)
        {
            return _repo.GetOne(email);
        }

        public User? Authenticate(string email, string pass)
        {
            return _repo.GetOne(email, pass);
        }
        public List<User> GetAllUsers()
        {
            return _repo.getAll();
        }
        public void AddUser(User user)
        {
            _repo.AddNew(user);
        }
        public void UpdateUser(User user)
        {
            _repo.Update(user);
        }
        public void DeleteUser(User user)
        {
            _repo.Delete(user);
        }
        public int AutoGenerateMemberID()
        {
            return _repo.AutoGenerateMemberID();
        }
    }
}
