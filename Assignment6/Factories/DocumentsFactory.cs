using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.Factories
{
    public abstract class DocumentsFactory
    {
        public Dictionary<string, PendingDocuments> DocumentsByRole = new Dictionary<string, PendingDocuments>();
        public abstract PendingDocuments this[string role] { get; }
    }

    public class DefaultPendingDocuments : DocumentsFactory
    {
        public Dictionary<string, PendingDocuments> pendingDocumentsByRole = new Dictionary<string, PendingDocuments>();

        public DefaultPendingDocuments(ApplicationDbContext db, int userId)
        {

            pendingDocumentsByRole.Add(Roles.Analyst.ToString(), new AnalystPendingDocuments(db, userId, Roles.Analyst));
            pendingDocumentsByRole.Add(Roles.Architect.ToString(), new ArchitectPendingDocuments(db, userId, Roles.Architect));
            pendingDocumentsByRole.Add(Roles.Programmer.ToString(), new ProgrammerPendingDocuments(db, userId, Roles.Programmer));
            pendingDocumentsByRole.Add(Roles.Tester.ToString(), new TesterPendingDocuments(db, userId, Roles.Tester));

        }
        public override PendingDocuments this[string role]
        {
            get
            {
                return pendingDocumentsByRole[role];
            }
        }
    }

    public class DefaultCompletedDocuments : DocumentsFactory
    {


        public DefaultCompletedDocuments(ApplicationDbContext db, int userId)
        {

            DocumentsByRole.Add(Roles.Analyst.ToString(), new CompletedDocuments(db, userId, Roles.Analyst));
            DocumentsByRole.Add(Roles.Architect.ToString(), new CompletedDocuments(db, userId, Roles.Architect));
            DocumentsByRole.Add(Roles.Programmer.ToString(), new CompletedDocuments(db, userId, Roles.Programmer));
            DocumentsByRole.Add(Roles.Tester.ToString(), new CompletedDocuments(db, userId, Roles.Tester));

        }
        public override PendingDocuments this[string role]
        {
            get
            {
                return DocumentsByRole[role];
            }
        }
    }

}
