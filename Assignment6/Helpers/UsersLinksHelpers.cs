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
            if (user == null)
            {
                return html;
            }

            html += GetDropdownLinks("Pending", user.Roles);
            html += GetDropdownLinks("Completed", user.Roles);

            return html;
        }
        private static string GetDropdownLinks(string task, IEnumerable<Role> roles)
        {
            string html =
                "<li class='nav-item dropdown'>" +
                $"<a class='nav-link dropdown-toggle' href='#' id='navbarDropdown{task}' role='button' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>" +
                $"{task} Tasks" +
                "</a>" +
                $"<div class='dropdown-menu' aria-labelledby='navbarDropdown{task}'>";

            foreach (var role in roles)
            {
                html += $"<a class='dropdown-item' href='/Home/Tasks/{task}/{role.Name}'>{role.Name}</a>";
            }

            html += "</div></li>";
            return html;
        }
    }
}

