using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;

namespace Assignment6.Models
{

    public class DocumentAssignManager : TableManager<DocumentAssign>
    {
        public DocumentAssignManager(ApplicationDbContext db)
        {
            _queryParts = new Dictionary<string, string>()
            {
                { "FindById", "DocumentAssign.Id = @Id" },
                { "InsertQuery",
                    "INSERT INTO DocumentAssign ([DocumentId], [AssignedToRoleId], [PurchasedByUserId], [Status]) " +
                    "VALUES (@DocumentId, @AssignedToRoleId, @PurchasedByUserId, @Status) " +
                    "SELECT * FROM DocumentAssign WHERE DocumentAssign.Id = (SELECT SCOPE_IDENTITY())"},
                { "RemoveQuery",
                    "DELETE FROM DocumentAssign WHERE Id = @Id" },
                { "UpdateQuery",
                    "UPDATE DocumentAssign SET " +
                    "[DocumentId]=@DocumentId, [AssignedToRoleId]=@AssignedToRoleId, [PurchasedByUserId]=@PurchasedByUserId, [Status]=@Status " +
                    "WHERE Id = @Id"}
            };
            _db = db;
        }
        public override IEnumerable<DocumentAssign> Get(string queryWhere = null, object parameters = null)
        {
            IEnumerable<DocumentAssign> documentAssigns = new List<DocumentAssign>();
            _db.UsingConnection((dbCon) =>
            {
                var documentAssignsDictionary = new Dictionary<int, DocumentAssign>();

                documentAssigns = dbCon.Query<DocumentAssign, Document, Role, User, DocumentAssign>(
                    "SELECT DocumentAssign.*, [Document].*, Role.*, [User].Id, [User].Username " +
                    "FROM DocumentAssign " +
                    "INNER JOIN [Document] ON DocumentAssign.DocumentId = [Document].Id " +
                    "INNER JOIN Role ON DocumentAssign.AssignedToRoleId = Role.Id " +
                    "LEFT JOIN [User] ON DocumentAssign.PurchasedByUserId = [User].Id " +
                    (queryWhere == null ? string.Empty : $" WHERE {queryWhere}"),
                    (documentAssign, document, role, user) =>
                    {
                        DocumentAssign documentAssignEntry;

                        if (!documentAssignsDictionary.TryGetValue(documentAssign.Id, out documentAssignEntry))
                        {
                            documentAssignEntry = documentAssign;
                            documentAssignEntry.PurchasedByUser = user;
                            documentAssignEntry.AssignedToRole = role;
                            documentAssignEntry.Document = document;
                            documentAssignsDictionary.Add(documentAssign.Id, documentAssignEntry);
                        }

                        return documentAssignEntry;
                    },
                    splitOn: "id",
                    param: parameters).Distinct();
            });

            return documentAssigns;
        }

        public bool Complete(int id, int documentId, int byUserId, int roleId)
        {
            if (!Find(id, out DocumentAssign documentAssign))
            {
                return false;
            }

            // If that is not current user AssignedDocument record then
            // create a new one purchashed by current user and role
            // with completed status
            if ((documentAssign.Status == "Completed" && documentAssign.PurchasedByUserId != byUserId) ||
                (documentAssign.Status == "Completed" && documentAssign.AssignedToRoleId != roleId))
            {
                return Add(new DocumentAssign()
                {
                    AssignedToRoleId = roleId,
                    PurchasedByUserId = byUserId,
                    DocumentId = documentId,
                    Status = "Completed"

                }) != null;

            }

            // update completed status and purchashed user 
            documentAssign.Status = "Completed";
            documentAssign.PurchasedByUserId = byUserId;

            return Update(documentAssign);
        }
    }
}

