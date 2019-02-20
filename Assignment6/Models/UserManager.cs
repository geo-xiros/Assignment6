using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Assignment6.Models
{
    public class UserManager
    {
        ApplicationDbContext _db;
        public UserManager(ApplicationDbContext db)
        {
            _db = db;
        }
        public User Login(string username, string password)
        {
            var loggedInUser = _db.User.FirstOrDefault(u => u.Username == username && u.Password == password);
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

                foreach (var role in loggedInUser.Role)
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
            return _db.User.Any(u => u.Username.Equals(username, System.StringComparison.InvariantCultureIgnoreCase));
        }

        public bool Register(User user)
        {
            bool success;

            _db.User.Add(user);
            _db.SaveChanges();
            success = true;

            return success;
        }
    }
}