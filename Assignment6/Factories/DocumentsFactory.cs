using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.Factories
{
    public class DefaultPendingDocuments
    {
        public Dictionary<string, PendingDocuments> pendingDocumentsByRole;

        public DefaultPendingDocuments(int userId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            pendingDocumentsByRole = new Dictionary<string, PendingDocuments>();

            pendingDocumentsByRole.Add(Roles.Analyst.ToString(), new AnalystPendingDocuments(db, userId));
            pendingDocumentsByRole.Add(Roles.Architect.ToString(), new ArchitectPendingDocuments(db, userId));
            pendingDocumentsByRole.Add(Roles.Programmer.ToString(), new ProgrammerPendingDocuments(db, userId));
            pendingDocumentsByRole.Add(Roles.Tester.ToString(), new TesterPendingDocuments(db, userId));

        }
        public PendingDocuments this[string role]
        {
            get
            {
                return pendingDocumentsByRole[role];
            }
        }
    }
    public abstract class PendingDocuments
    {
        protected ApplicationDbContext _db;
        protected int _userId;
        public abstract IEnumerable<Document> GetDocuments();
    }

    public class AnalystPendingDocuments : PendingDocuments
    {
        public AnalystPendingDocuments(ApplicationDbContext db, int userId)
        {
            _db = db;
            _userId = userId;
        }
        public override IEnumerable<Document> GetDocuments()
        {
            return _db.Documents
                .Get()
                .Where(d => d.AssignedDocuments.Any(a => (a.AssignedToRoleId == (int)Roles.Analyst && a.Status == "Pending" && a.PurchasedByUserId == _userId)));
        }
    }

    public class ArchitectPendingDocuments : PendingDocuments
    {
        public ArchitectPendingDocuments(ApplicationDbContext db, int userId)
        {
            _db = db;
            _userId = userId;
        }
        public override IEnumerable<Document> GetDocuments()
        {
            var result = _db.Documents
                .Get()
                .Where(d =>
                    d.AssignedDocuments.Any(a =>
                         (a.AssignedToRoleId == (int)Roles.Analyst && a.Status == "Completed") ||
                         (a.AssignedToRoleId == (int)Roles.Architect && a.Status == "Pending" && a.PurchasedByUserId == _userId))
                    ).Distinct()
                .ToList();
            result.RemoveAll(d => d.AssignedDocuments.Any(a => a.AssignedToRoleId == (int)Roles.Architect && a.Status == "Completed"));

            return result;
        }
    }

    public class ProgrammerPendingDocuments : PendingDocuments
    {
        public ProgrammerPendingDocuments(ApplicationDbContext db, int userId)
        {
            _db = db;
            _userId = userId;
        }
        public override IEnumerable<Document> GetDocuments()
        {
            var result = _db.Documents
                .Get()
                .Where(d =>
                    d.AssignedDocuments.Any(a =>
                         (a.AssignedToRoleId == (int)Roles.Architect && a.Status == "Completed") ||
                         (a.AssignedToRoleId == (int)Roles.Programmer && a.Status == "Pending" && a.PurchasedByUserId == _userId))
                    ).Distinct()
                .ToList();
            result.RemoveAll(d => d.AssignedDocuments.Any(a =>
                (a.AssignedToRoleId == (int)Roles.Programmer && a.Status == "Completed") ||
                (a.AssignedToRoleId == (int)Roles.Analyst && a.Status == "Pending")));

            return result;
        }
    }

    public class TesterPendingDocuments : PendingDocuments
    {
        public TesterPendingDocuments(ApplicationDbContext db, int userId)
        {
            _db = db;
            _userId = userId;
        }
        public override IEnumerable<Document> GetDocuments()
        {
            var result = _db.Documents
                .Get()
                .Where(d =>
                    d.AssignedDocuments.Any(a =>
                         (a.AssignedToRoleId == (int)Roles.Programmer && a.Status == "Completed") ||
                         (a.AssignedToRoleId == (int)Roles.Tester && a.Status == "Pending" && a.PurchasedByUserId == _userId))
                    ).Distinct()
                .ToList();
            result.RemoveAll(d => d.AssignedDocuments.Any(a =>
                (a.AssignedToRoleId == (int)Roles.Tester && a.Status == "Completed") ||
                (a.AssignedToRoleId == (int)Roles.Analyst && a.Status == "Pending") ||
                (a.AssignedToRoleId == (int)Roles.Architect && a.Status == "Pending")));

            return result;

        }
    }


}


//static void Main(string[] args)
//{
//    List<Doc> docs = new List<Doc>()
//            {
//                new Doc(){
//                    id = 1,
//                    completedTasks = new Dictionary<int, bool>()
//                    {
//                        { 1, true },
//                        { 2, true },
//                        { 3, true },
//                        { 4, false }
//                    }
//                },
//                new Doc(){
//                    id = 2,
//                    completedTasks = new Dictionary<int, bool>()
//                    {
//                        { 1, false },
//                        { 2, true },
//                        { 3, true },
//                        { 4, true }
//                    }
//                }

//            };

//    var x1 = docs.Where(d => d.completedTasks.Where(SecondRoleTasks).Count() == 2);
//    var x2 = docs.Where(d => d.completedTasks.Where(ThirdRoleTasks).Count() == 3);
//    var x3 = docs.Where(d => d.completedTasks.Where(FourthRoleTasks).Count() == 3);

//    Console.WriteLine($"x1={x1.FirstOrDefault()?.id}, count={x1.Count()}");
//    Console.WriteLine($"x2={x2.FirstOrDefault()?.id}, count={x2.Count()}");
//    Console.WriteLine($"x3={x3.FirstOrDefault()?.id}, count={x3.Count()}");

//    Console.ReadKey();
//}
//public class Doc
//{
//    public int id { get; set; }
//    public Dictionary<int, bool> completedTasks;
//}

//public static bool SecondRoleTasks(KeyValuePair<int, bool> role)
//{
//    return (role.Key == 1 && role.Value == true) ||
//           (role.Key == 2 && role.Value == false);
//}
//public static bool ThirdRoleTasks(KeyValuePair<int, bool> role)
//{
//    return (role.Key == 1 && role.Value == true) ||
//           (role.Key == 2 && role.Value == true) ||
//           (role.Key == 3 && role.Value == false);
//}
//public static bool FourthRoleTasks(KeyValuePair<int, bool> role)
//{
//    return (role.Key == 1 && role.Value == true) ||
//           (role.Key == 2 && role.Value == true) ||
//           (role.Key == 3 && role.Value == true) ||
//           (role.Key == 4 && role.Value == false);
//}