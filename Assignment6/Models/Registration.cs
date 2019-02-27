using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Assignment6.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
        public IEnumerable<Role> Roles { get; set; }

        public int? RegisteredByUserId { get; set; }

        [DisplayName("Registered By")]
        public string RegisteredByUsername { get; set; }

        [DisplayName("Registration Status")]
        public string  Status { get; set; }
        
    }

}