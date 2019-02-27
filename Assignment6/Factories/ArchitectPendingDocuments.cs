using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.Factories
{
    public class ArchitectPendingDocuments : PendingDocuments
    {
        public ArchitectPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 2) { }

        protected override bool DocumentsToPurchase(KeyValuePair<int, int> task)
        {
            return (task.Key == (int)Roles.Analyst && (task.Value == 1)) ||
                   (task.Key == (int)Roles.Architect && task.Value == 0);
        }

    }


}
