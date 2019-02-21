using Assignment6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment6.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext _db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Manager")]
        public ActionResult Manager()
        {
            var usersToRegister = _db.Users.Get().Where(u => u.Roles.Any(r => r.Registered == false));
            return View(usersToRegister);

        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status">Pending, Completed</param>
        /// <param name="role">Manager, Developer...</param>
        /// <returns></returns>
        public ActionResult Tasks(string status,string role)
        {
            if (!HttpContext.User.IsInRole(role))
            {
                return Redirect("/Home");
            }

            RegistrationManager registrationManager = new RegistrationManager(_db);

            IEnumerable<Registration> usersRegistrations = registrationManager.Get("Status=@status", new { status });
            ViewBag.Title = $"{status} Tasks for {role}";
            ViewBag.Status = status;
            ViewBag.Registered = status == "Completed";
            return View("UsersRegistrations", usersRegistrations);
        }
    }
}

