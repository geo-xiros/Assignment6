namespace Assignment6.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Document : IEqualityComparer<Document>
    {
        public Document()
        {
            AssignedDocuments = new List<DocumentAssign>();
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
