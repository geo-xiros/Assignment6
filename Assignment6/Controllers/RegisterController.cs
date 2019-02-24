using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Assignment6.Models;

namespace Assignment6.Controllers
{
    public class RegisterController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        public ActionResult Index()
        {
            ViewBag.RoleId = new SelectList(_db.Roles.Get(), "Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult Register(Registration registrationUser)
        {
            RegistrationFacade registrationFacade = new RegistrationFacade(_db);

            ViewBag.Title = registrationFacade.Register(registrationUser);
            return View("Message");
        }

    }
    public class RegistrationFacade
    {
        private ApplicationDbContext _db;
        public RegistrationFacade(ApplicationDbContext db)
        {
            _db = db;
        }
        public string Register(Registration registrationUser)
        {

            if (!_db.Users.UserExists(registrationUser.Username, out User user))
            {
                user = _db.Users.Add(
                    new Models.User()
                    {
                        Username = registrationUser.Username,
                        Password = registrationUser.Password
                    });

            }
            else
            {
                if (!_db.Users.ValidateUser(registrationUser.Username, registrationUser.Password, out user))
                {
                    return $"Wrong User Password.";
                }
            }

            string message;

            if (RoleAllreadyApproved(user, registrationUser.RoleId, out message))
            {
                return message;
            }

            Role role = _db.Roles.Get("Id=@RoleId", new { registrationUser.RoleId  }).FirstOrDefault();

            if (RoleAllreadyPending(user, role, out message))
            {
                return message;
            }

            registrationUser.UserId = user.Id;
            registrationUser.Status = "Pending";

            _db.Registrations.Add(registrationUser);

            return $"User registered Pending Role {role.Name} to be approved";
        }

        private bool RoleAllreadyApproved(User user, int roleId, out string message)
        {
            message = string.Empty;

            Role role = user.Roles.FirstOrDefault(r => r.Id == roleId);
            if (role != null)
            {
                message = $"Allready Approved As {role.Name}";
                return true;
            }

            return false;
        }
        private bool RoleAllreadyPending(User user, Role role, out string message)
        {
            message = string.Empty;

            if (_db.Registrations.Get("RegisteredByUserId Is Null").Any(u =>
                u.UserId == user.Id &&
                u.RoleId == role.Id))
            {
                
                message = $"Allready Pending Role {role.Name} to be approved";
                return true;
            }

            return false;
        }
    }
}