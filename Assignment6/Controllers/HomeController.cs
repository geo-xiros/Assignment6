﻿using Assignment6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Assignment6.ViewModels;

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
            ViewBag.Role = role;
            ViewBag.Pending = status == "Pending";

            if (role == "Manager")
            {
                return View("UsersRegistrations", usersRegistrations.OrderBy(r => r.UserId).ThenBy(r => r.RoleId));
            }
            User loggedUser = Session["user"] as User;
            if (loggedUser != null)
            {

                var RoleId = loggedUser.Roles.FirstOrDefault(r => r.Name.Equals(role))?.Id ?? 0;

                UserTaskView userTaskView = new UserTaskView()
                {
                    UserId = loggedUser.Id,
                    RoleId = loggedUser.Roles.FirstOrDefault(r => r.Name.Equals(role))?.Id ?? 0
                };
                string statusFilter = status == "Pending" ? "Status = 'Pending'" : "Status <> 'Pending'";
                userTaskView.DocumentAssigns = _db
                    .DocumentAssigns
                    .Get(statusFilter + " AND ((PurchasedByUserId=@UserId AND AssignedToRoleId=@RoleId) OR (PurchasedByUserId Is null AND AssignedToRoleId=@RoleId))", new
                    {
                        UserId = loggedUser.Id,
                        RoleId
                    });

                return View("UserTasks", userTaskView);
            }

            return View();
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

        public ActionResult Complete(int id, int roleId, int userId)
        {
            if (_db.DocumentAssigns.CompletedBy(id, userId))
            {
                int assignToRoleId = roleId + 1;
                _db.DocumentAssigns.ForwardToNextRole(id, assignToRoleId);
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}

