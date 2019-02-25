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
            NextRoleFactory nextRoleFactory = new FirstVersionRoleManagerFactory();

            foreach (var role in loggedInUser.Roles)
            {
                var nextRoleManager = nextRoleFactory.MakeNextRoleManager(role.Name);
                Session[$"NextRoleManager{role.Id}"] = nextRoleManager;
            }
            Session["DefaultPendingDocuments"] = new DefaultPendingDocuments(loggedInUser.Id);

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
    public class DefaultPendingDocuments
    {
        public Dictionary<string, PendingDocuments> pendingDocumentsByRole;

        public DefaultPendingDocuments(int userId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            pendingDocumentsByRole = new Dictionary<string, PendingDocuments>();

            pendingDocumentsByRole.Add(Roles.Analyst.ToString(), new AnalystPendingDocuments(db, userId));
            pendingDocumentsByRole.Add(Roles.Architect.ToString(), new ArchitectPendingDocuments(db, userId));
            pendingDocumentsByRole.Add(Roles.Programmer.ToString(), new ProgrammerPendingDocuments(db, userId));
            pendingDocumentsByRole.Add(Roles.Tester.ToString(), new TesterPendingDocuments(db, userId));

        }
        public PendingDocuments this[string role]
        {
            get
            {
                return pendingDocumentsByRole[role];
            }
        }
    }
    public abstract class PendingDocuments
    {
        protected ApplicationDbContext _db;
        protected int _userId;
        public abstract IEnumerable<Document> GetDocuments();
    }

    public class AnalystPendingDocuments : PendingDocuments
    {
        public AnalystPendingDocuments(ApplicationDbContext db, int userId)
        {
            _db = db;
            _userId = userId;
        }
        public override IEnumerable<Document> GetDocuments()
        {
            return _db.Documents
                .Get()
                .Where(d=> d.AssignedDocuments.Any(a => (a.AssignedToRoleId == (int)Roles.Analyst && a.Status == "Pending" && a.PurchasedByUserId == _userId)));
        }
    }

    public class ArchitectPendingDocuments : PendingDocuments
    {
        public ArchitectPendingDocuments(ApplicationDbContext db, int userId)
        {
            _db = db;
            _userId = userId;
        }
        public override IEnumerable<Document> GetDocuments()
        {
            var result = _db.Documents
                .Get()
                .Where(d =>
                    d.AssignedDocuments.Any(a =>
                         (a.AssignedToRoleId == (int)Roles.Analyst && a.Status == "Completed") ||
                         (a.AssignedToRoleId == (int)Roles.Architect && a.Status == "Pending" && a.PurchasedByUserId == _userId))
                    ).Distinct()
                .ToList();
            result.RemoveAll(d => d.AssignedDocuments.Any(a => a.AssignedToRoleId == (int)Roles.Architect && a.Status == "Completed"));

            return result;
        }
    }

    public class ProgrammerPendingDocuments : PendingDocuments
    {
        public ProgrammerPendingDocuments(ApplicationDbContext db, int userId)
        {
            _db = db;
            _userId = userId;
        }
        public override IEnumerable<Document> GetDocuments()
        {
            var result = _db.Documents
                .Get()
                .Where(d =>
                    d.AssignedDocuments.Any(a =>
                         (a.AssignedToRoleId == (int)Roles.Architect && a.Status == "Completed") ||
                         (a.AssignedToRoleId == (int)Roles.Programmer && a.Status == "Pending" && a.PurchasedByUserId == _userId))
                    ).Distinct()
                .ToList();
            result.RemoveAll(d => d.AssignedDocuments.Any(a => 
                (a.AssignedToRoleId == (int)Roles.Programmer && a.Status == "Completed") ||
                (a.AssignedToRoleId == (int)Roles.Analyst && a.Status == "Pending")));

            return result;
        }
    }

    public class TesterPendingDocuments : PendingDocuments
    {
        public TesterPendingDocuments(ApplicationDbContext db, int userId)
        {
            _db = db;
            _userId = userId;
        }
        public override IEnumerable<Document> GetDocuments()
        {
            var result = _db.Documents
                .Get()
                .Where(d =>
                    d.AssignedDocuments.Any(a =>
                         (a.AssignedToRoleId == (int)Roles.Programmer && a.Status == "Completed") ||
                         (a.AssignedToRoleId == (int)Roles.Tester && a.Status == "Pending" && a.PurchasedByUserId == _userId))
                    ).Distinct()
                .ToList();
            result.RemoveAll(d => d.AssignedDocuments.Any(a =>
                (a.AssignedToRoleId == (int)Roles.Tester && a.Status == "Completed") ||
                (a.AssignedToRoleId == (int)Roles.Analyst && a.Status == "Pending") ||
                (a.AssignedToRoleId == (int)Roles.Architect && a.Status == "Pending")));

            return result;

        }
    }
}