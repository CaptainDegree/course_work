using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyShareTermApp.TestModels
{
    [Table("money_mailer")]
    public partial class MoneyMailer
    {
        public MoneyMailer()
        {
            Commentary = new HashSet<Commentary>();
            Message = new HashSet<Message>();
            MoneyTransferPerson = new HashSet<MoneyTransfer>();
            MoneyTransferTarget = new HashSet<MoneyTransfer>();
            Person = new HashSet<Person>();
            Post = new HashSet<Post>();
            Subscription = new HashSet<Subscription>();
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("creation_time")]
        public DateTime CreationTime { get; set; }

        [InverseProperty("Mailer")]
        public virtual ICollection<Commentary> Commentary { get; set; }
        [InverseProperty("Mailer")]
        public virtual ICollection<Message> Message { get; set; }
        [InverseProperty("Person")]
        public virtual ICollection<MoneyTransfer> MoneyTransferPerson { get; set; }
        [InverseProperty("Target")]
        public virtual ICollection<MoneyTransfer> MoneyTransferTarget { get; set; }
        [InverseProperty("Mailer")]
        public virtual ICollection<Person> Person { get; set; }
        [InverseProperty("Mailer")]
        public virtual ICollection<Post> Post { get; set; }
        [InverseProperty("Mailer")]
        public virtual ICollection<Subscription> Subscription { get; set; }
    }
}
