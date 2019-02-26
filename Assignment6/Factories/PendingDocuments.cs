using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.Factories
{

    public abstract class PendingDocuments
    {
        protected ApplicationDbContext _db;
        protected int _userId;
        protected int _role;
        protected int _maxCountOfDocumentsForCreteria;

        public PendingDocuments(ApplicationDbContext db, int userId, Roles role, int maxCountOfDocumentsForCreteria)
        {
            _db = db;
            _userId = userId;
            _role = (int)role;
            _maxCountOfDocumentsForCreteria = maxCountOfDocumentsForCreteria;
        }

        public IEnumerable<Document> GetDocuments()
        {
            return _db.Documents
                .Get()
                .Where(d =>
                    d.IsCompletedByRole.Where(CompletedDocuments).Count() == _maxCountOfDocumentsForCreteria ||
                    d.AssignedDocuments.Any(PendingOwnedTasks));
        }

        protected abstract bool CompletedDocuments(KeyValuePair<int, int> task);

        private bool PendingOwnedTasks(DocumentAssign documentAssign)
        {
            return documentAssign.AssignedToRoleId == _role &&
                documentAssign.PurchasedByUserId == _userId &&
                documentAssign.Status == "Pending";
        }
    }

}
