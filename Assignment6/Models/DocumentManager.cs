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
                documents = dbCon.Query<Document>(
                    "SELECT * FROM Document " +
                    (queryWhere == null ? string.Empty : $" WHERE {queryWhere}"),
                    parameters);
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