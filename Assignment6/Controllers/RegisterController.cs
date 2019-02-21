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
        private UserManager userManager;
        private RoleManager roleManager;
        private RegistrationManager registrationManager;
        public RegistrationFacade(ApplicationDbContext db)
        {
            _db = db;
            userManager = new UserManager(_db);
            roleManager = new RoleManager(_db);
            registrationManager = new RegistrationManager(_db);
        }
        public string Register(Registration registrationUser)
        {

            if (!userManager.UserExists(registrationUser.Username, out User user))
            {
                user = userManager.Add(
                    new Models.User()
                    {
                        Username = registrationUser.Username,
                        Password = registrationUser.Password
                    });

            }

            Role role = user.Roles.FirstOrDefault(r => r.Id == registrationUser.RoleId);
            if (role != null)
            {
                return $"Allready Approved As {role.Name}";
            }

            role = roleManager.Get("Id=@RoleId", new { registrationUser.RoleId }).FirstOrDefault();

            if (registrationManager.Get().Any(u =>
                u.UserId == user.Id &&
                u.RoleId == registrationUser.RoleId &&
                u.Status.Equals("Pending")))
            {
                return $"Allready Pending Role {role.Name} to be approved";
            }

            registrationUser.UserId = user.Id;
            registrationUser.RegisteredAt = DateTime.Now;
            registrationUser.Status = "Pending";

            registrationManager.Add(registrationUser);

            return $"User registered Pending Role {role.Name} to be approved";
        }
    }
}