using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;

namespace Assignment6.Models
{
    public class DocumentManager : TableManager<Document>
    {
        public DocumentManager(ApplicationDbContext db)
        {
            _queryParts = new Dictionary<string, string>()
            {
                { "FindById", "Document.Id = @Id" },
                { "InsertQuery",
                    "INSERT INTO Document ([Title], [Body]) " +
                    "VALUES (@Title, @Body) " +
                    "SELECT * FROM Document WHERE Document.Id = (SELECT SCOPE_IDENTITY())"},
                { "RemoveQuery",
                    "DELETE FROM Document WHERE Id = @Id" },
                { "UpdateQuery",
                    "UPDATE Document SET " +
                    "[Title]=@Title, [Body]=@Body " +
                    "WHERE Id = @Id"}
            };
            _db = db;
        }
        public override IEnumerable<Document> Get(string queryWhere = null, object parameters = null)
        {
            IEnumerable<Document> documents = new List<Document>();

            _db.UsingConnection((dbCon) =>
            {
                var DocumentsDictionary = new Dictionary<int, Document>();
                var documentAssignsDictionary = new Dictionary<int, DocumentAssign>();

                documents = dbCon.Query<Document, DocumentAssign, Role, User, Document>(
                    "SELECT [Document].*, DocumentAssign.*, Role.*, [User].Id, [User].Username  " +
                    "FROM [Document] " +
                    "INNER JOIN DocumentAssign ON [Document].Id = DocumentAssign.DocumentId " +
                    "INNER JOIN Role ON DocumentAssign.AssignedToRoleId = Role.Id " +
                    "LEFT JOIN [User] ON DocumentAssign.PurchasedByUserId = [User].Id " +
                    (queryWhere == null ? string.Empty : $" WHERE {queryWhere}"),
                    (document, documentAssign, role, user) =>
                    {
                        Document documentEntry;

                        if (!DocumentsDictionary.TryGetValue(document.Id, out documentEntry))
                        {
                            documentEntry = document;
                            documentEntry.AssignedDocuments = new List<DocumentAssign>();
                            DocumentsDictionary.Add(document.Id, document);
                        }

                        DocumentAssign documentAssignEntry;

                        if (!documentAssignsDictionary.TryGetValue(documentAssign.Id, out documentAssignEntry))
                        {
                            documentAssignEntry = documentAssign;
                            documentAssignEntry.PurchasedByUser = user;
                            documentAssignEntry.AssignedToRole = role;
                            documentAssignEntry.Document = documentEntry;
                            documentAssignsDictionary.Add(documentAssign.Id, documentAssign);
                        }

                        if (documentAssignEntry != null)
                        {
                            documentEntry.IsCompletedByRole[documentAssignEntry.AssignedToRoleId] = documentAssignEntry.Status == "Completed" ? 1:2;
                            documentEntry.AssignedDocuments.Add(documentAssignEntry);
                        }

                        return documentEntry;
                    },
                    splitOn: "id",
                    param: parameters).Distinct();
            });

            return documents;
        }
        public Document Add(string title, string body)
        {
            return Add(new Document()
            {
                Title = title,
                Body = body
            });
        }
    }
}