using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.Helpers
{
    public class RegistrationFacade
    {
        private ApplicationDbContext _db;
        public RegistrationFacade(ApplicationDbContext db)
        {
            _db = db;
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

            Role role = _db.Roles.Get("Id=@RoleId", new { registrationUser.RoleId }).FirstOrDefault();

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

            if (_db.Registrations.Get("RegisteredByUserId Is Null").Any(u =>
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