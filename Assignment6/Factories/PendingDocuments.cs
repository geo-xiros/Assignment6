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
        protected override bool DocumentsToPurchase(KeyValuePair<int, DocumentAssignStatus> assignedDocumentState)
        {
            return (assignedDocumentState.Key == 0);
        }
    }

    public class ArchitectPendingDocuments : DocumentsRepository
    {
        public ArchitectPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 2) { }

        protected override bool DocumentsToPurchase(KeyValuePair<int, DocumentAssignStatus> assignedDocumentState)
        {
            return (assignedDocumentState.Key == (int)Roles.Analyst && (assignedDocumentState.Value == DocumentAssignStatus.Completed)) ||
                   (assignedDocumentState.Key == (int)Roles.Architect && assignedDocumentState.Value == 0);
        }

    }

    public class ProgrammerPendingDocuments : DocumentsRepository
    {
        public ProgrammerPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 3) { }

        protected override bool DocumentsToPurchase(KeyValuePair<int, DocumentAssignStatus> assignedDocumentState)
        {
            return (assignedDocumentState.Key == (int)Roles.Analyst && (assignedDocumentState.Value == DocumentAssignStatus.Completed || assignedDocumentState.Value == DocumentAssignStatus.NotAssigned)) ||
                   (assignedDocumentState.Key == (int)Roles.Architect && (assignedDocumentState.Value == DocumentAssignStatus.Completed)) ||
                   (assignedDocumentState.Key == (int)Roles.Programmer && assignedDocumentState.Value == DocumentAssignStatus.NotAssigned);
        }

    }
    
    public class TesterPendingDocuments : DocumentsRepository
    {
        public TesterPendingDocuments(ApplicationDbContext db, int userId, Roles role) : base(db, userId, role, 4) { }

        protected override bool DocumentsToPurchase(KeyValuePair<int, DocumentAssignStatus> assignedDocumentState)
        {
            return (assignedDocumentState.Key == (int)Roles.Analyst && (assignedDocumentState.Value == DocumentAssignStatus.Completed || assignedDocumentState.Value == DocumentAssignStatus.NotAssigned)) ||
                   (assignedDocumentState.Key == (int)Roles.Architect && (assignedDocumentState.Value == DocumentAssignStatus.Completed || assignedDocumentState.Value == DocumentAssignStatus.NotAssigned)) ||
                   (assignedDocumentState.Key == (int)Roles.Programmer && (assignedDocumentState.Value == DocumentAssignStatus.Completed)) ||
                   (assignedDocumentState.Key == (int)Roles.Tester && assignedDocumentState.Value == DocumentAssignStatus.NotAssigned);
        }

    }
}
