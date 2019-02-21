using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment6.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string  Status { get; set; }
    }

}