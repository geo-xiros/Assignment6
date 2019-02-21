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
            UserManager userManager = new UserManager(_db);
            User user;
            if (!userManager.UserExists(registrationUser.Username, out user))
            {
                user = userManager.Add(
                    new Models.User()
                    {
                        Username = registrationUser.Username,
                        Password = registrationUser.Password
                    });

            }

            Role role = user.Roles.Where(r => r.Id == registrationUser.RoleId).FirstOrDefault();
            if (role != null)
            {
                return View($"Allready Approved As {role.Name}");
            }

            RegistrationManager registrationManager = new RegistrationManager(_db);
            if (registrationManager.Get().Any(u=> 
                u.UserId==user.Id && 
                u.RoleId==registrationUser.RoleId && 
                u.Status.Equals("Pending")))
            {
                return View($"Allready Pending");
            }

            registrationUser.UserId = user.Id;
            registrationUser.RegisteredAt = DateTime.Now;
            registrationUser.Status = "Pending";

            registrationManager.Add(registrationUser);

            return RedirectToAction("Index", "Home");
        }

    }
}