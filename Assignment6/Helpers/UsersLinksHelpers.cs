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
        public static string UsersLinks(this HtmlHelper helper)
        {
            string html = string.Empty;
            User user = helper.ViewContext.HttpContext.Session["User"] as User;

            if (user == null)
            {
                return html;
            }
            
            html += GetDropdownLinks("Pending", user.Roles);
            html += GetDropdownLinks("Completed", user.Roles);

            return html;
        }
        private static string GetDropdownLinks(string status, IEnumerable<Role> roles)
        {
            string html =
                "<li class='nav-item dropdown'>" +
                $"<a class='nav-link dropdown-toggle' href='#' id='navbarDropdown{status}' role='button' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>" +
                $"{status} Tasks" +
                "</a>" +
                $"<div class='dropdown-menu' aria-labelledby='navbarDropdown{status}'>";

            foreach (var role in roles)
            {
                html += $"<a class='dropdown-item' href='/Home/Tasks/{status}/{role.Name}'>{role.Name}</a>";
            }

            html += "</div></li>";
            return html;
        }
    }
}

