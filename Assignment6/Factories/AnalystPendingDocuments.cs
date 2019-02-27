using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.Factories
{
    
    public class AnalystPendingDocuments : PendingDocuments
    {
        public AnalystPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 1) { }
        protected override bool DocumentsToPurchase(KeyValuePair<int, int> task)
        {
            return (task.Key == 0);
        }
    }

}
