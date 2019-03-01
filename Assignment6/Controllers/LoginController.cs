using Assignment6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Assignment6.Factories;

namespace Assignment6.Controllers
{
    public class LoginController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            UserManager userManager = new UserManager(_db);
            User loggedInUser;
            if (!userManager.UserExists(user.Username, out loggedInUser))
            {
                ModelState.AddModelError("Username", "Wrong username or password.");
                return View("Index", user);
            }
            if (!userManager.ValidateUser(user.Username, user.Password, out loggedInUser))
            {
                ModelState.AddModelError("Username", "Wrong username or password.");
                return View("Index", user);
            }

            // TODO
            // Check if has at least one Approved role 
            //if (!HasApprovedRole)
            //{
            //    ModelState.AddModelError("Username", "Pending Role to be approved.");
            //    return View("Index", user);
            //}
            
            var documents = new DocumentsFactory(_db, loggedInUser.Id);

            Session["user"] = loggedInUser;
            Session["PendingDocuments"] = documents.GetPending();
            Session["CompletedDocuments"] = documents.GetCompleted();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Request.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}