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
        public bool Register(Registration registrationUser, out string message)
        {

            if (!_db.Users.UserExists(registrationUser.Username, out User user))
            {
                user = _db.Users.Add(
                    new Models.User()
                    {
                        Username = registrationUser.Username,
                        Password = registrationUser.Password
                    });

            }
            else
            {
                if (!_db.Users.ValidateUser(registrationUser.Username, registrationUser.Password, out user))
                {
                    message = $"Wrong User Password.";
                    return false;
                }
            }

            if (RoleAllreadyApproved(user, registrationUser.RoleId, out message))
            {
                return false;
            }

            Role role = _db.Roles.Find( registrationUser.RoleId );

            if (RoleAllreadyPending(user, role, out message))
            {
                return false;
            }

            registrationUser.UserId = user.Id;
            registrationUser.Status = "Pending";

            _db.Registrations.Add(registrationUser);

            message = $"User registered as {role.Name}. Approval is pending.";
            return true;
        }

        private bool RoleAllreadyApproved(User user, int roleId, out string message)
        {
            message = string.Empty;

            Role role = user.Roles.FirstOrDefault(r => r.Id == roleId);
            if (role != null)
            {
                message = $"Allready approved as {role.Name}";
                return true;
            }

            return false;
        }
        private bool RoleAllreadyPending(User user, Role role, out string message)
        {
            message = string.Empty;

            if (Get("RegisteredByUserId Is Null").Any(u =>
                u.UserId == user.Id &&
                u.RoleId == role.Id))
            {

                message = $"Allready registered as {role.Name}. Approval is pending.";
                return true;
            }

            return false;
        }

    }

}