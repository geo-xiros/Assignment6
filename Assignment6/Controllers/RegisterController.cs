using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Assignment6.Models;
using Assignment6.Helpers;
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
            string message = string.Empty;

            if (!registrationFacade.Register(registrationUser, out message))
            {
                ViewBag.RoleId = new SelectList(_db.Roles.Get(), "Id", "Name");
                ModelState.AddModelError("Username", message);
                return View("Index", registrationUser);
            }
            TempData["RegistrationInfo"] = message;
            
            return RedirectToAction("Index","Home" );
        }

    }
}