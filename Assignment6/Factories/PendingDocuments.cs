using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.Factories
{



    public class AnalystPendingDocuments : DocumentsManager
    {
        public AnalystPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 1) { }
        protected override bool DocumentsToPurchase(KeyValuePair<int, int> task)
        {
            return (task.Key == 0);
        }
    }

    public class ArchitectPendingDocuments : DocumentsManager
    {
        public ArchitectPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 2) { }

        protected override bool DocumentsToPurchase(KeyValuePair<int, int> task)
        {
            return (task.Key == (int)Roles.Analyst && (task.Value == 1)) ||
                   (task.Key == (int)Roles.Architect && task.Value == 0);
        }

    }

    public class ProgrammerPendingDocuments : DocumentsManager
    {
        public ProgrammerPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 3) { }

        protected override bool DocumentsToPurchase(KeyValuePair<int, int> task)
        {
            return (task.Key == (int)Roles.Analyst && (task.Value == 1 || task.Value == 0)) ||
                   (task.Key == (int)Roles.Architect && (task.Value == 1)) ||
                   (task.Key == (int)Roles.Programmer && task.Value == 0);
        }

    }

    public class TesterPendingDocuments : DocumentsManager
    {
        public TesterPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 4) { }

        protected override bool DocumentsToPurchase(KeyValuePair<int, int> task)
        {
            return (task.Key == (int)Roles.Analyst && (task.Value == 1 || task.Value == 0)) ||
                   (task.Key == (int)Roles.Architect && (task.Value == 1 || task.Value == 0)) ||
                   (task.Key == (int)Roles.Programmer && (task.Value == 1)) ||
                   (task.Key == (int)Roles.Tester && task.Value == 0);
        }

    }
}
