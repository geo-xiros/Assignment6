namespace Assignment6.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Document : IEqualityComparer<Document>
    {
        public Dictionary<int, int> IsCompletedByRole { get; set; }

        public Document()
        {
            AssignedDocuments = new List<DocumentAssign>();
            IsCompletedByRole = new Dictionary<int, int>()
            {
                { (int)Roles.Analyst,0 },
                { (int)Roles.Architect,0 },
                { (int)Roles.Programmer,0 },
                { (int)Roles.Tester,0 }
            };
        }
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        public string Body { get; set; }
        public List<DocumentAssign> AssignedDocuments { get; set; }

        public bool Equals(Document x, Document y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Document obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
