using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
namespace Assignment6.Models
{
    public class RoleManager : TableManager<Role>
    {
        public RoleManager(ApplicationDbContext db)
        {
            _queryParts = new Dictionary<string, string>()
            {
                { "FindById", "Role.Id = @id" },
                { "InsertQuery",
                    "INSERT INTO Role ([Name]) " +
                    "VALUES (@Name)" +
                    "SELECT * FROM [Role] WHERE Role.Id = @id)"},
                { "RemoveQuery",
                    "DELETE FROM [Role] WHERE Id = @Id" },
                { "UpdateQuery",
                    "UPDATE [Role] SET " +
                    "[Name]=@Name " +
                    "WHERE Id = @Id"}
            };
            _db = db;
        }
        public override IEnumerable<Role> Get(string queryWhere = null, object parameters = null)
        {
            IEnumerable<Role> roles = new List<Role>();

            _db.UsingConnection((dbCon) =>
            {
                roles = dbCon.Query<Role>(
                    "SELECT * FROM Role" + (queryWhere == null ? string.Empty : $" WHERE {queryWhere}"),
                    parameters);
            });

            return roles;
        }


    }

}