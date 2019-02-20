using Assignment6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment6.ViewModels
{
    public class UsersRegistrations
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public bool Registered { get; set; }

        public virtual ICollection<Role> Role { get; set; }
    }
}