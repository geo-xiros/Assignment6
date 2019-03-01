using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.Factories
{

    public class DocumentsFactory
    {
        private ApplicationDbContext _db;
        private int _userId;
        public DocumentsFactory(ApplicationDbContext db, int userId)
        {
            _db = db;
            _userId = userId;
        }
        public Dictionary<string, DocumentsRepository> GetPending()
        {
            return new Dictionary<string, DocumentsRepository>()
            {
                { Roles.Analyst.ToString(), new AnalystPendingDocuments(_db, _userId, Roles.Analyst) },
                { Roles.Architect.ToString(), new ArchitectPendingDocuments(_db, _userId, Roles.Architect) },
                { Roles.Programmer.ToString(), new ProgrammerPendingDocuments(_db, _userId, Roles.Programmer)},
                { Roles.Tester.ToString(), new TesterPendingDocuments(_db, _userId, Roles.Tester)}
            };
        }

        public Dictionary<string, DocumentsRepository> GetCompleted()
        {
            return new Dictionary<string, DocumentsRepository>()
            {
                { Roles.Analyst.ToString(), new CompletedDocuments(_db, _userId, Roles.Analyst) },
                { Roles.Architect.ToString(), new CompletedDocuments(_db, _userId, Roles.Architect) },
                { Roles.Programmer.ToString(), new CompletedDocuments(_db, _userId, Roles.Programmer)},
                { Roles.Tester.ToString(), new CompletedDocuments(_db, _userId, Roles.Tester)}
            };
        }
    }
      
}
