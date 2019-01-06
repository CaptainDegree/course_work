using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyShareTermApp.TestModels
{
    [Table("currency_set")]
    public partial class CurrencySet
    {
        public CurrencySet()
        {
            MoneyTransfer = new HashSet<MoneyTransfer>();
            PersonAccount = new HashSet<Person>();
            PersonCommentPrice = new HashSet<Person>();
            PersonMessagePrice = new HashSet<Person>();
            PersonSubscriptionPrice = new HashSet<Person>();
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("euro", TypeName = "numeric")]
        public decimal Euro { get; set; }
        [Column("ruble", TypeName = "numeric")]
        public decimal Ruble { get; set; }
        [Column("dollar", TypeName = "numeric")]
        public decimal Dollar { get; set; }

        [InverseProperty("Account")]
        public virtual ICollection<MoneyTransfer> MoneyTransfer { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<Person> PersonAccount { get; set; }
        [InverseProperty("CommentPrice")]
        public virtual ICollection<Person> PersonCommentPrice { get; set; }
        [InverseProperty("MessagePrice")]
        public virtual ICollection<Person> PersonMessagePrice { get; set; }
        [InverseProperty("SubscriptionPrice")]
        public virtual ICollection<Person> PersonSubscriptionPrice { get; set; }
    }
}
