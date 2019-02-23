using Assignment6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            UserManager manager = new UserManager(_db);
            User loggedInUser;

            if (!manager.Login(user.Username, user.Password, out loggedInUser))
            {
                if (loggedInUser == null)
                {
                    ModelState.AddModelError("Username", "Wrong username or password.");
                }
                else
                {
                    ModelState.AddModelError("Username", "Pending user role approval.");
                }

                return View("Index", user);
            }

            Session["user"] = loggedInUser;
            return View("Success", loggedInUser);
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