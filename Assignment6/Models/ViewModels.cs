using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Assignment6.Models
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
                Role = db.Role.Where(r => r.Id == this.RoleId).ToList()
            };
        }
    }
}