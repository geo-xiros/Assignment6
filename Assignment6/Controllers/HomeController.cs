using Assignment6.Models;
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
                var usersRegistrations = _db
                    .Registrations
                    .Get(status == "Pending" ? "RegisteredByUserId Is Null" : "RegisteredByUserId Is Not Null")
                    .OrderBy(r => r.UserId)
                    .ThenBy(r => r.RoleId);
                return View("UsersRegistrations", usersRegistrations);
            }

            User loggedUser = Session["user"] as User;
            DefaultPendingDocuments defaultPendingDocuments = Session["DefaultPendingDocuments"] as DefaultPendingDocuments;

            if (loggedUser != null && defaultPendingDocuments != null)
            {

                var RoleId = loggedUser.Roles.FirstOrDefault(r => r.Name.Equals(role))?.Id ?? 0;

                UserTaskView userTaskView = new UserTaskView()
                {
                    UserId = loggedUser.Id,
                    RoleId = loggedUser.Roles.FirstOrDefault(r => r.Name.Equals(role))?.Id ?? 0
                };

                // TODO:
                // Completed Documents 
                if (status == "Pending")
                {
                    PendingDocuments pendingDocuments = defaultPendingDocuments[role];

                    userTaskView.Documents = pendingDocuments.GetDocuments();

                }
                else
                {
                    userTaskView.Documents = _db.Documents
                        .Get()
                        .Where(d =>
                            d.AssignedDocuments.Any(a =>
                                a.PurchasedByUserId == loggedUser.Id && 
                                a.AssignedToRoleId == RoleId && 
                                a.Status=="Completed"));
                }

                //string statusFilter = status == "Pending" ? "Status = 'Pending'" : "Status <> 'Pending'";
                //userTaskView.DocumentAssigns = _db
                //    .DocumentAssigns
                //    .Get(statusFilter + " AND ((PurchasedByUserId=@UserId AND AssignedToRoleId=@RoleId) OR (PurchasedByUserId Is null AND AssignedToRoleId=@RoleId))", new
                //    {
                //        UserId = loggedUser.Id,
                //        RoleId
                //    });

                return View("UserTasks", userTaskView);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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

        [Authorize(Roles = "Architect,Analyst,Programmer,Tester")]
        public ActionResult Complete(int id, int? documentAssignId, int roleId, int userId)
        {
            DocumentAssign documentAssign = _db.DocumentAssigns.Get("DocumentAssign.Id = @documentAssignId", new { documentAssignId }).FirstOrDefault();

            if (documentAssignId == null)
            {
                documentAssign = new DocumentAssign()
                {
                    PurchasedByUserId = userId,
                    AssignedToRoleId = roleId,
                    Status = "Completed",
                    DocumentId = id
                };
                _db.DocumentAssigns.Add(documentAssign);

            }
            else
            {
                documentAssign.Status = "Completed";
                documentAssign.PurchasedByUserId = userId;
                _db.DocumentAssigns.Update(documentAssign);
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
    }

}

