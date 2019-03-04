using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.Factories
{

    public abstract class DocumentsRepository
    {
        protected ApplicationDbContext _db;
        protected int _userId;
        protected int _roleId;
        protected int _maxCountOfDocumentsForCreteria;

        public DocumentsRepository(ApplicationDbContext db, int userId, Roles role, int maxCountOfDocumentsForCreteria)
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
                    d.AssignedDocuments.Any(PendingAssignedDocuments));
        }

        protected virtual bool DocumentsToPurchase(KeyValuePair<int, DocumentAssignStatus> task)
        {
            return false;
        }

        private bool PendingAssignedDocuments(DocumentAssign documentAssign)
        {
            return documentAssign.AssignedToRoleId == _roleId &&
                documentAssign.PurchasedByUserId == _userId &&
                documentAssign.Status == "Pending";
        }
    }

}
