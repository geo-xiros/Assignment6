using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.ViewModels
{
    public class RegisterUser
    {

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        public bool Registered { get; set; }

        public int RoleId { get; set; }
        //public virtual ICollection<Role> Role { get; set; }

        public User GetUser(ApplicationDbContext db)
        {
            return new User()
            {
                Username = this.Username,
                Password = this.Password,
                Roles = db.Roles.Get().Where(r => r.Id == this.RoleId).ToList()
            };
        }
    }
}