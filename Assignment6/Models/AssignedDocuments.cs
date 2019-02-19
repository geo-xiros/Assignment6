namespace Assignment6.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AssignedDocuments
    {
        public int Id { get; set; }

        public int DocumentId { get; set; }

        public int RoleId { get; set; }

        public bool Completed { get; set; }

        public virtual Document Document { get; set; }

        public virtual Role Role { get; set; }
    }
}
