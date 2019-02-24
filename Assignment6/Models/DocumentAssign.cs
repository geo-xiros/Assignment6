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
        public Document Document { get; set; }

        public int AssignedToRoleId { get; set; }
        public Role AssignedToRole { get; set; }

        public int? PurchasedByUserId { get; set; }

        public User PurchasedByUser { get; set; }

        public string Status { get; set; }

    }
}
