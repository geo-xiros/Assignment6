using Assignment6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment6.Factories
{
    public class CompletedDocuments : PendingDocuments
    {
        public CompletedDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 0) { }

        public override IEnumerable<Document> GetDocuments()
        {
            return _db.Documents
                .Get("AssignedToRoleId=@_roleId And PurchasedByUserId=@_userId AND Status='Completed'",
                new
                {
                    _roleId,
                    _userId
                });
        }

    }
}