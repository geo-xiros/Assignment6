using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Dapper;

namespace Assignment6.Models
{
    public class RegistrationManager : TableManager<Registration>
    {
        public RegistrationManager(ApplicationDbContext db)
        {
            _queryParts = new Dictionary<string, string>()
            {
                { "FindById", "Registration.Id = @Id" },
                { "InsertQuery",
                    "INSERT INTO Registration ([UserId],[RoleId],[RegisteredByUserId],[Status]) " +
                    "VALUES (@UserId,@RoleId,@RegisteredByUserId,@Status) " +
                    "SELECT * FROM Registration WHERE Registration.Id = (SELECT SCOPE_IDENTITY())"},
                { "RemoveQuery",
                    "DELETE FROM Registration WHERE Id = @Id" },
                { "UpdateQuery",
                    "UPDATE Registration SET " +
                    "[UserId]=@UserId,[RoleId]=@RoleId,[RegisteredByUserId]=@RegisteredByUserId,[Status]=@Status " +
                    "WHERE Id = @Id"}
            };
            _db = db;
        }
        public override IEnumerable<Registration> Get(string queryWhere = null, object parameters = null)
        {
            IEnumerable<Registration> registrations = new List<Registration>();

            _db.UsingConnection((dbCon) =>
            {
                registrations = dbCon.Query<Registration>(
                    "SELECT Registration.*, [User].Username, Role.Name AS Role, Manager.Username AS RegisteredByUsername " +
                    "FROM Registration " +
                    "INNER JOIN [User] ON Registration.UserId = [User].Id " +
                    "INNER JOIN Role ON Registration.RoleId = Role.Id " +
                    "LEFT JOIN [User] Manager ON Registration.RegisteredByUserId = Manager.Id " +
                    (queryWhere == null ? string.Empty : $" WHERE {queryWhere}"),
                    parameters);
            });

            return registrations;
        }
        public bool Approve(int id, int byUserId)
        {
            return UpdateRegistrationStatus(id, byUserId, "Approved");
        }
        public bool Decline(int id, int byUserId)
        {
            return UpdateRegistrationStatus(id, byUserId, "Declined");
        }
        private bool UpdateRegistrationStatus(int id, int byUserId, string status)
        {
            if (!Find(id, out Registration registration))
            {

                return false;
            }

            registration.RegisteredByUserId = byUserId;
            registration.Status = status;

            return Update(registration);
        }
    }

}