using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.Factories
{
    public abstract class DocumentsFactory
    {
        protected Dictionary<string, DocumentsManager> _documentsByRole = new Dictionary<string, DocumentsManager>();
        public abstract DocumentsManager this[string role] { get; }
    }

    public class DefaultPendingDocuments : DocumentsFactory
    {

        public DefaultPendingDocuments(ApplicationDbContext db, int userId)
        {

            _documentsByRole.Add(Roles.Analyst.ToString(), new AnalystPendingDocuments(db, userId, Roles.Analyst));
            _documentsByRole.Add(Roles.Architect.ToString(), new ArchitectPendingDocuments(db, userId, Roles.Architect));
            _documentsByRole.Add(Roles.Programmer.ToString(), new ProgrammerPendingDocuments(db, userId, Roles.Programmer));
            _documentsByRole.Add(Roles.Tester.ToString(), new TesterPendingDocuments(db, userId, Roles.Tester));

        }
        public override DocumentsManager this[string role]
        {
            get
            {
                return _documentsByRole[role];
            }
        }
    }

    public class DefaultCompletedDocuments : DocumentsFactory
    {


        public DefaultCompletedDocuments(ApplicationDbContext db, int userId)
        {

            _documentsByRole.Add(Roles.Analyst.ToString(), new CompletedDocuments(db, userId, Roles.Analyst));
            _documentsByRole.Add(Roles.Architect.ToString(), new CompletedDocuments(db, userId, Roles.Architect));
            _documentsByRole.Add(Roles.Programmer.ToString(), new CompletedDocuments(db, userId, Roles.Programmer));
            _documentsByRole.Add(Roles.Tester.ToString(), new CompletedDocuments(db, userId, Roles.Tester));

        }
        public override DocumentsManager this[string role]
        {
            get
            {
                return _documentsByRole[role];
            }
        }
    }

    public abstract class DocumentsManager
    {
        protected ApplicationDbContext _db;
        protected int _userId;
        protected int _roleId;
        protected int _maxCountOfDocumentsForCreteria;

        public DocumentsManager(ApplicationDbContext db, int userId, Roles role, int maxCountOfDocumentsForCreteria)
        {
            _db = db;
            _userId = userId;
            _roleId = (int)role;
            _maxCountOfDocumentsForCreteria = maxCountOfDocumentsForCreteria;
        }

        public virtual IEnumerable<Document> Get()
        {
            return _db.Documents
                .Get()
                .Where(d =>
                    d.IsCompletedByRole.Where(DocumentsToPurchase).Count() == _maxCountOfDocumentsForCreteria ||
                    d.AssignedDocuments.Any(PendingOwnedTasks));
        }

        protected virtual bool DocumentsToPurchase(KeyValuePair<int, int> task)
        {
            return false;
        }

        private bool PendingOwnedTasks(DocumentAssign documentAssign)
        {
            return documentAssign.AssignedToRoleId == _roleId &&
                documentAssign.PurchasedByUserId == _userId &&
                documentAssign.Status == "Pending";
        }
    }

}
