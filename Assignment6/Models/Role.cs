namespace Assignment6.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public  class Role
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public bool Registered { get; set; }

    }
}
