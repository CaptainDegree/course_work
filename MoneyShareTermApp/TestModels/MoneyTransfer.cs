using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyShareTermApp.TestModels
{
    [Table("money_transfer")]
    public partial class MoneyTransfer
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("person_id")]
        public int PersonId { get; set; }
        [Column("target_id")]
        public int TargetId { get; set; }
        [Column("account_id")]
        public int AccountId { get; set; }
        [Column("time")]
        public DateTime Time { get; set; }
        [Column("type")]
        public char Type { get; set; }

        [ForeignKey("AccountId")]
        [InverseProperty("MoneyTransfer")]
        public virtual CurrencySet Account { get; set; }
        [ForeignKey("PersonId")]
        [InverseProperty("MoneyTransferPerson")]
        public virtual MoneyMailer Person { get; set; }
        [ForeignKey("TargetId")]
        [InverseProperty("MoneyTransferTarget")]
        public virtual MoneyMailer Target { get; set; }
    }
}
