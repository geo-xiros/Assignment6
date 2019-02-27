using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.Factories
{

    public class ProgrammerPendingDocuments : PendingDocuments
    {
        public ProgrammerPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 3) { }

        protected override bool DocumentsToPurchase(KeyValuePair<int, int> task)
        {
            return (task.Key == (int)Roles.Analyst && (task.Value == 1 || task.Value == 0)) ||
                   (task.Key == (int)Roles.Architect && (task.Value == 1)) ||
                   (task.Key == (int)Roles.Programmer && task.Value == 0);
        }

    }


}
