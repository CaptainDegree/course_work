using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyShareTermApp.Models
{
    [Table("money_mailer")]
    public partial class MoneyMailer
    {
        public MoneyMailer()
        {
            Commentary = new HashSet<Commentary>();
            Message = new HashSet<Message>();
            Person = new HashSet<Person>();
            Post = new HashSet<Post>();
            Subscription = new HashSet<Subscription>();
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("creation_time", TypeName = "time without time zone")]
        public TimeSpan CreationTime { get; set; }

        [InverseProperty("Mailer")]
        public virtual ICollection<Commentary> Commentary { get; set; }
        [InverseProperty("Mailer")]
        public virtual ICollection<Message> Message { get; set; }
        [InverseProperty("Mailer")]
        public virtual ICollection<Person> Person { get; set; }
        [InverseProperty("Mailer")]
        public virtual ICollection<Post> Post { get; set; }
        [InverseProperty("Mailer")]
        public virtual ICollection<Subscription> Subscription { get; set; }
    }
}
