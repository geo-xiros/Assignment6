using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.Factories
{

    public class AnalystPendingDocuments : DocumentsRepository
    {
        public AnalystPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 1) { }
        protected override bool DocumentsToPurchase(KeyValuePair<int, DocumentAssignStatus> task)
        {
            return (task.Key == 0);
        }
    }

    public class ArchitectPendingDocuments : DocumentsRepository
    {
        public ArchitectPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 2) { }

        protected override bool DocumentsToPurchase(KeyValuePair<int, DocumentAssignStatus> task)
        {
            return (task.Key == (int)Roles.Analyst && (task.Value == DocumentAssignStatus.Completed)) ||
                   (task.Key == (int)Roles.Architect && task.Value == 0);
        }

    }

    public class ProgrammerPendingDocuments : DocumentsRepository
    {
        public ProgrammerPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 3) { }

        protected override bool DocumentsToPurchase(KeyValuePair<int, DocumentAssignStatus> task)
        {
            return (task.Key == (int)Roles.Analyst && (task.Value == DocumentAssignStatus.Completed || task.Value == DocumentAssignStatus.NotAssigned)) ||
                   (task.Key == (int)Roles.Architect && (task.Value == DocumentAssignStatus.Completed)) ||
                   (task.Key == (int)Roles.Programmer && task.Value == DocumentAssignStatus.NotAssigned);
        }

    }
    
    public class TesterPendingDocuments : DocumentsRepository
    {
        public TesterPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 4) { }

        protected override bool DocumentsToPurchase(KeyValuePair<int, DocumentAssignStatus> task)
        {
            return (task.Key == (int)Roles.Analyst && (task.Value == DocumentAssignStatus.Completed || task.Value == DocumentAssignStatus.NotAssigned)) ||
                   (task.Key == (int)Roles.Architect && (task.Value == DocumentAssignStatus.Completed || task.Value == DocumentAssignStatus.NotAssigned)) ||
                   (task.Key == (int)Roles.Programmer && (task.Value == DocumentAssignStatus.Completed)) ||
                   (task.Key == (int)Roles.Tester && task.Value == DocumentAssignStatus.NotAssigned);
        }

    }
}
