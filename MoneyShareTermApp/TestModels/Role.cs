using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyShareTermApp.TestModels
{
    [Table("role")]
    public partial class Role
    {
        public Role()
        {
            Person = new HashSet<Person>();
        }

        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; }

        [InverseProperty("Role")]
        public virtual ICollection<Person> Person { get; set; }
    }
}
