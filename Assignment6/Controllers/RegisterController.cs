﻿using System;
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
            ViewBag.RoleId = new SelectList(_db.Role, "Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterUser registerUser)
        {
            UserManager manager = new UserManager(_db);
            manager.Register(registerUser.GetUser(_db));

            return RedirectToAction("Index","Home");
        }

    }
}