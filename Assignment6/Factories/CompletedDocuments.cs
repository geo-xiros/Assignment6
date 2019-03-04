using Assignment6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment6.Factories
{
    public class CompletedDocuments : DocumentsRepository
    {
        public CompletedDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 0) { }

        public override IEnumerable<Document> Get()
        {
            return _db.Documents
                .Get("AssignedToRoleId=@_roleId AND " +
                    "PurchasedByUserId=@_userId AND " +
                    "Status='Completed'",
                new
                {
                    _roleId,
                    _userId
                });
        }

    }
}