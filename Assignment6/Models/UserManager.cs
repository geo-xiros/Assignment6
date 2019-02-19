using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Assignment6.Models
{
    public class UserManager
    {
        ApplicationDbContext _db;
        public UserManager(ApplicationDbContext db)
        {
            _db = db;
        }
        public User Login(string username, string password)
        {
            return _db.User.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public bool UserExists(string username)
        {
            return _db.User.Any(u => u.Username == username);
        }
    }
}