using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment6.Models;

namespace Assignment6.ViewModels
{
    public class UserTaskView
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public IEnumerable<DocumentAssign> DocumentAssigns { get; set; }
        public IEnumerable<Document> Documents { get; set; }
    }
}