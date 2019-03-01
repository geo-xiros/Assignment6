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

            if (!userManager.Login(user.Username, user.Password, out User loggedInUser))
            {
                ModelState.AddModelError("Username",
                    loggedInUser == null ?
                    "Wrong username or password." :
                    "Pending user role approval.");

                return View("Index", user);
            }

            var documents= new DocumentsFactory(_db, loggedInUser.Id);

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