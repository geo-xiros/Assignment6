using Assignment6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment6.ViewModels
{
    public class UserRegistration
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string Status { get; set; }
    }
}