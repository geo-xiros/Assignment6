using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.Factories
{
    public class DefaultPendingDocuments
    {
        public Dictionary<string, PendingDocuments> pendingDocumentsByRole = new Dictionary<string, PendingDocuments>();

        public DefaultPendingDocuments(ApplicationDbContext db, int userId)
        {

            pendingDocumentsByRole.Add(Roles.Analyst.ToString(), new AnalystPendingDocuments(db, userId, Roles.Analyst));
            pendingDocumentsByRole.Add(Roles.Architect.ToString(), new ArchitectPendingDocuments(db, userId, Roles.Architect));
            pendingDocumentsByRole.Add(Roles.Programmer.ToString(), new ProgrammerPendingDocuments(db, userId, Roles.Programmer));
            pendingDocumentsByRole.Add(Roles.Tester.ToString(), new TesterPendingDocuments(db, userId, Roles.Tester));

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
        protected int _role;
        public PendingDocuments(ApplicationDbContext db, int userId, Roles role)
        {
            _db = db;
            _userId = userId;
            _role = (int)role;
        }
        public abstract IEnumerable<Document> GetDocuments();
        protected bool PendingOwnedTasks(DocumentAssign documentAssign)
        {
            return documentAssign.AssignedToRoleId == _role &&
                documentAssign.PurchasedByUserId == _userId &&
                documentAssign.Status == "Pending";
        }
    }

    public class AnalystPendingDocuments : PendingDocuments
    {
        public AnalystPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role) { }
        public override IEnumerable<Document> GetDocuments()
        {
            return _db.Documents
                .Get()
                .Where(d =>
                    d.IsCompletedByRole.Where(CompletedTasks).Count() == 1 ||
                    d.AssignedDocuments.Any(PendingOwnedTasks));
        }
        private bool CompletedTasks(KeyValuePair<int, int> task)
        {
            return (task.Key == 0);
        }
    }

    public class ArchitectPendingDocuments : PendingDocuments
    {
        public ArchitectPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role) { }
        public override IEnumerable<Document> GetDocuments()
        {
            return _db.Documents
                .Get()
                .Where(d =>
                    d.IsCompletedByRole.Where(CompletedTasks).Count() == 2 ||
                    d.AssignedDocuments.Any(PendingOwnedTasks));
        }
        private bool CompletedTasks(KeyValuePair<int, int> task)
        {
            return (task.Key == (int)Roles.Analyst && (task.Value == 1)) ||
                   (task.Key == (int)Roles.Architect && task.Value == 0);
        }

    }

    public class ProgrammerPendingDocuments : PendingDocuments
    {
        public ProgrammerPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role) { }
        public override IEnumerable<Document> GetDocuments()
        {
            return _db.Documents
                .Get()
                .Where(d =>
                    d.IsCompletedByRole.Where(CompletedTasks).Count() == 3 ||
                    d.AssignedDocuments.Any(PendingOwnedTasks));
        }
        private bool CompletedTasks(KeyValuePair<int, int> task)
        {
            return (task.Key == (int)Roles.Analyst && (task.Value == 1 || task.Value == 0)) ||
                   (task.Key == (int)Roles.Architect && (task.Value == 1 )) ||
                   (task.Key == (int)Roles.Programmer && task.Value == 0);
        }

    }

    public class TesterPendingDocuments : PendingDocuments
    {
        public TesterPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role) { }
        public override IEnumerable<Document> GetDocuments()
        {
            return _db.Documents
                .Get()
                .Where(d =>
                    d.IsCompletedByRole.Where(CompletedTasks).Count() == 4 ||
                    d.AssignedDocuments.Any(PendingOwnedTasks));
        }
        private bool CompletedTasks(KeyValuePair<int, int> task)
        {
            return (task.Key == (int)Roles.Analyst && (task.Value == 1|| task.Value == 0)) ||
                   (task.Key == (int)Roles.Architect && (task.Value == 1 || task.Value == 0)) ||
                   (task.Key == (int)Roles.Programmer && (task.Value == 1 )) ||
                   (task.Key == (int)Roles.Tester && task.Value == 0);
        }

    }

}
