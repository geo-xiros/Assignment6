using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.Factories
{

    public class TesterPendingDocuments : PendingDocuments
    {
        public TesterPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 4) { }

        protected override bool CompletedDocuments(KeyValuePair<int, int> task)
        {
            return (task.Key == (int)Roles.Analyst && (task.Value == 1 || task.Value == 0)) ||
                   (task.Key == (int)Roles.Architect && (task.Value == 1 || task.Value == 0)) ||
                   (task.Key == (int)Roles.Programmer && (task.Value == 1)) ||
                   (task.Key == (int)Roles.Tester && task.Value == 0);
        }

    }

}
