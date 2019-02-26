using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.Factories
{
    public class DefaultPendingDocuments
    {
        public Dictionary<string, PendingDocuments> pendingDocumentsByRole = new Dictionary<string, PendingDocuments>();

        public DefaultPendingDocuments(ApplicationDbContext db, int userId)
        {

            pendingDocumentsByRole.Add(Roles.Analyst.ToString(), new AnalystPendingDocuments(db, userId, Roles.Analyst));
            pendingDocumentsByRole.Add(Roles.Architect.ToString(), new ArchitectPendingDocuments(db, userId, Roles.Architect));
            pendingDocumentsByRole.Add(Roles.Programmer.ToString(), new ProgrammerPendingDocuments(db, userId, Roles.Programmer));
            pendingDocumentsByRole.Add(Roles.Tester.ToString(), new TesterPendingDocuments(db, userId, Roles.Tester));

        }
        public PendingDocuments this[string role]
        {
            get
            {
                return pendingDocumentsByRole[role];
            }
        }
    }

}
