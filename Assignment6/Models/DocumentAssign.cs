namespace Assignment6.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class DocumentAssign
    {
        public int Id { get; set; }

        public int DocumentId { get; set; }

        public int RoleId { get; set; }

        public int UserId { get; set; }

        //public bool Completed { get; set; }

    }
}
