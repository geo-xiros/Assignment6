using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Dapper;

namespace Assignment6.Models
{
    public class UserManager : TableManager<User>
    {
        public UserManager(ApplicationDbContext db)
        {
            _queryParts = new Dictionary<string, string>()
            {
                { "FindById", "[User].Id = @id" },
                { "InsertQuery",
                    "INSERT INTO [User] ([Username],[Password]) " +
                    "VALUES (@Username, @Password) " +
                    "SELECT [User].Id, [User].Username FROM [User] WHERE [User].Id = (SELECT SCOPE_IDENTITY())"},
                { "RemoveQuery",
                    "DELETE FROM [User] WHERE Id = @Id" },
                { "UpdateQuery",
                    "UPDATE [User] SET " +
                    "[Username]=@Username, [Password]=@Password " +
                    "WHERE Id = @Id"}
            };
            _db = db;
        }
        public override IEnumerable<User> Get(string queryWhere = null, object parameters = null)
        {
            IEnumerable<User> users = new List<User>();

            _db.UsingConnection((dbCon) =>
            {
                var userDictionary = new Dictionary<int, User>();
                users = dbCon.Query<User, Role, User>(
                    "SELECT [User].Id, [User].Username, Role.Id, Role.Name " +
                    "FROM [User] LEFT JOIN UserRoles ON [User].Id = UserRoles.UserId LEFT JOIN " +
                    " Role ON UserRoles.RoleId = Role.Id" + (queryWhere == null ? string.Empty : $" WHERE {queryWhere}"),
                    (user, role) =>
                    {
                        User userEntry;
                        if (!userDictionary.TryGetValue(user.Id, out userEntry))
                        {
                            userEntry = user;
                            userEntry.Roles = new List<Role>();
                            userDictionary.Add(user.Id, user);
                        }

                        if (role != null) userEntry.Roles.Add(role);

                        return userEntry;
                    },
                    splitOn: "id",
                    param: parameters)
                    .Distinct();
            });

            return users;
        }

        /// <summary>
        /// Finds a User using parameters Username and Password
        /// on success the user is authenticated
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="loggedInUser"></param>
        /// <returns>true if user is authenticated with at least <string>one role</string>strong></returns>
        public bool Login(string username, string password, out User loggedInUser)
        {

            if (!ValidateUser(username, password, out loggedInUser))
            {
                return false;
            }

            if (loggedInUser.Roles.Count == 0)
            {
                return false;
            }

            Authenticate(loggedInUser);

            return true;
        }
        private void Authenticate(User loggedInUser)
        {
            var claims = new List<Claim>(new[]
            {
                    // adding following 2 claim just for supporting default antiforgery provider
                    new Claim(ClaimTypes.NameIdentifier, loggedInUser.Username),
                    new Claim(
                        "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                        "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                    new Claim(ClaimTypes.Name, loggedInUser.Username)
                });

            foreach (var role in loggedInUser.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            HttpContext.Current.GetOwinContext().Authentication.SignIn(
                new AuthenticationProperties { IsPersistent = false }, identity);
        }
        public bool ValidateUser(string username, string password, out User user)
        {
            user = Get("Username=@username And Password=@password",
                new { username, password })
                .FirstOrDefault();
            return user != null;
        }
        public bool UserExists(string username, out User user)
        {
            user = Get("Username=@username", new { username }).FirstOrDefault();
            return user != null;
        }
        public bool AddUserRole(int userId, int roleId)
        {
            int affectedRows = 0;

            _db.UsingConnection((dbCon) =>
            {
                affectedRows = dbCon.Execute("INSERT INTO UserRoles ([UserId], [RoleId]) Values (@userId, @roleId)",
                    new
                    {
                        userId,
                        roleId
                    });
            });

            return affectedRows != 0;
        }
        public bool RemoveUserRole(int userId, int roleId)
        {
            int affectedRows = 0;

            _db.UsingConnection((dbCon) =>
            {
                affectedRows = dbCon.Execute("DELETE FROM UserRoles WHERE UserId=@userId AND RoleId=@roleId",
                    new
                    {
                        userId,
                        roleId
                    });
            });

            return affectedRows != 0;
        }
    }

}

