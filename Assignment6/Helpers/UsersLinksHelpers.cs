using Assignment6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment6.Helpers
{
    public static class UsersLinksHelpers
    {
        public static string UsersLinks(this HtmlHelper helper, User user)
        {
            string html = string.Empty;
            if (user==null)
            {
                return html;
            }

            foreach (var role in user.Roles)
            {
                html += $"<li class='nav-item'><a class='nav-link' href='/Home/{role.Name}'>{role.Name}</a></li>";
            }
            return html;
        }
    }
}