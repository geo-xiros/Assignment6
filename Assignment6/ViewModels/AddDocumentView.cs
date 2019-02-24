using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment6.ViewModels
{
    public class AddDocumentView
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}