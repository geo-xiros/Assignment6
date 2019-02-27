﻿using System;
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
        protected int _roleId;
        protected int _maxCountOfDocumentsForCreteria;

        public PendingDocuments(ApplicationDbContext db, int userId, Roles role, int maxCountOfDocumentsForCreteria)
        {
            _db = db;
            _userId = userId;
            _roleId = (int)role;
            _maxCountOfDocumentsForCreteria = maxCountOfDocumentsForCreteria;
        }

        public virtual IEnumerable<Document> GetDocuments()
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
