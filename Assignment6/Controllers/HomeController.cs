﻿using Assignment6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Assignment6.ViewModels;
using System.Net;
using Assignment6.Factories;

namespace Assignment6.Controllers
{

    public class HomeController : Controller
    {
        ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult Index()
        {
            ViewBag.RegistrationInfo = TempData["RegistrationInfo"]?.ToString();
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

            ViewBag.Title = $"{status} Tasks for {role}";
            ViewBag.Status = status;
            ViewBag.Role = role;
            ViewBag.Pending = status == "Pending";

            if (role == "Manager")
            {
                return ManagerView(status);
            }

            return UsersView(status, role);

        }

        private ActionResult UsersView(string status, string role)
        {
            User loggedUser = Session["user"] as User;
            var defaultDocuments = Session[$"{status}Documents"] as Dictionary<string, DocumentsRepository>;

            if (loggedUser == null || defaultDocuments == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var RoleId = loggedUser.Roles.FirstOrDefault(r => r.Name.Equals(role))?.Id ?? 0;

            UserTaskView userTaskView = new UserTaskView()
            {
                UserId = loggedUser.Id,
                RoleId = RoleId
            };

            DocumentsRepository pendingDocuments = defaultDocuments[role];
            userTaskView.Documents = pendingDocuments.Get();


            return View("UserTasks", userTaskView);
        }

        private ActionResult ManagerView(string status)
        {
            var usersRegistrations = _db
                .Registrations
                .Get(status == "Pending" ? "RegisteredByUserId Is Null" : "RegisteredByUserId Is Not Null")
                .OrderBy(r => r.UserId)
                .ThenBy(r => r.RoleId);
            return View("UsersRegistrations", usersRegistrations);
        }

        [Authorize(Roles = "Manager")]
        public ActionResult Approve(int? id)
        {
            User loggedUser = Session["user"] as User;

            if (!_db.Registrations.Approve(id ?? 0, loggedUser.Id))
            {
                // TODO 
                // Error Handling
                return Redirect(Request.UrlReferrer.ToString());
            }

            if (_db.Registrations.Find(id ?? 0, out Registration registration))
            {
                _db.Users.AddUserRole(registration.UserId, registration.RoleId);
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        [Authorize(Roles = "Manager")]
        public ActionResult Decline(int? id)
        {
            User loggedUser = Session["user"] as User;
            if (!_db.Registrations.Decline(id ?? 0, loggedUser.Id))
            {
                // TODO 
                // Error Handling
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        [Authorize(Roles = "Architect,Analyst,Programmer,Tester")]
        public ActionResult Complete(int id, int? documentAssignId, int roleId, int userId)
        {
            if (!_db.DocumentAssigns.Complete(documentAssignId ?? 0, id, userId, roleId))
            {
                // TODO 
                // Update Error Handling
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

    }

}

