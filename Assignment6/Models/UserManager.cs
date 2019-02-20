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
                { "FindById", "User.Id = @id" },
                { "InsertQuery",
                    "INSERT INTO User ([Username],[Password]) " +
                    "VALUES (@Username, @Password)" +
                    "SELECT * FROM [User] WHERE User.Id = @id)"},
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
                    "SELECT [User].Id, [User].Username, [User].Password, Role.Id, Role.Name, UserRoles.Registered " +
                    "FROM [User] INNER JOIN UserRoles ON[User].Id = UserRoles.UserId INNER JOIN " +
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

                        userEntry.Roles.Add(role);

                        return userEntry;
                    },
                    splitOn: "id",
                    param: parameters)
                    .Distinct();
            });

            return users;
        }

        public User Login(string username, string password)
        {
            var loggedInUser = _db.Users.Get()
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (loggedInUser != null)
            {
                var claims = new List<Claim>(new[]
                {
                    // adding following 2 claim just for supporting default antiforgery provider
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(
                        "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                        "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                    new Claim(ClaimTypes.Name, username)
                });

                foreach (var role in loggedInUser.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }

                var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                HttpContext.Current.GetOwinContext().Authentication.SignIn(
                    new AuthenticationProperties { IsPersistent = false }, identity);
            }
            return loggedInUser;
        }

        public bool UserExists(string username)
        {
            return _db.Users.Get().Any(u => u.Username.Equals(username, System.StringComparison.InvariantCultureIgnoreCase));
        }

        public bool Register(User user)
        {
            bool success;

            _db.Users.Add(user);
            success = true;

            return success;
        }
    }

}