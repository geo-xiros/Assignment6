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
        public ActionResult Tasks(string status, string role)
        {
            if (!HttpContext.User.IsInRole(role))
            {
                return Redirect("/Home");
            }

            var usersRegistrations = GetRegistrations(status == "Pending" ? "Status='Pending'" : "Status<>'Pending'");

            ViewBag.Title = $"{status} Tasks for {role}";
            ViewBag.Status = status;
            ViewBag.Pending = status == "Pending";

            return View("UsersRegistrations", usersRegistrations);
        }

        public ActionResult Approve(int? id)
        {
            Registration registration = _db.Registrations.Get("Registration.Id=@id", new { id }).FirstOrDefault();
            registration.Status = "Approved";
            _db.Registrations.Update(registration);

            // TODO
            // Insert UserRoles Record with UserId, RoleId

            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult Decline(int? id)
        {
            Registration registration = _db.Registrations.Get("Registration.Id=@id", new { id }).FirstOrDefault();
            registration.Status = "Declined";
            _db.Registrations.Update(registration);

            // TODO
            // Insert UserRoles Record with UserId, RoleId
            return Redirect(Request.UrlReferrer.ToString());
        }

        private IEnumerable<Registration> GetRegistrations(string statusFilter)
        {
            RegistrationManager registrationManager = new RegistrationManager(_db);

            return registrationManager.Get(statusFilter);

        }
    }
}

