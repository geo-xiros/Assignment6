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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status">Pending, Completed</param>
        /// <param name="role">Manager, Developer...</param>
        /// <returns></returns>
        [Authorize(Roles = "Manager,Architect,Analyst,Programmer,Tester")]
        public ActionResult Tasks(string status, string role)
        {

            var usersRegistrations = _db.Registrations.Get(status == "Pending" ? "RegisteredByUserId Is Null" : "RegisteredByUserId Is Not Null");

            ViewBag.Title = $"{status} Tasks for {role}";
            ViewBag.Status = status;
            ViewBag.Pending = status == "Pending";

            return View("UsersRegistrations", usersRegistrations.OrderBy(r => r.UserId).ThenBy(r => r.RoleId));
        }

        [Authorize(Roles = "Manager")]
        public ActionResult Approve(int? id)
        {

            User loggedUser = Session["user"] as User;

            Registration registration = _db.Registrations.Get("Registration.Id=@id", new { id }).FirstOrDefault();
            registration.Status = "Approved";
            registration.RegisteredByUserId = loggedUser.Id;
            _db.Registrations.Update(registration);

            _db.Users.AddUserRole(registration.UserId, registration.RoleId);

            return Redirect(Request.UrlReferrer.ToString());
        }

        [Authorize(Roles = "Manager")]
        public ActionResult Decline(int? id)
        {
            User loggedUser = Session["user"] as User;

            Registration registration = _db.Registrations.Get("Registration.Id=@id", new { id }).FirstOrDefault();
            registration.Status = "Declined";
            registration.RegisteredByUserId = loggedUser.Id;
            _db.Registrations.Update(registration);

            return Redirect(Request.UrlReferrer.ToString());
        }


    }
}

