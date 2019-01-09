using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace MoneyShareTermApp.Models
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
        public int Type { get; set; }

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

    public partial class MoneyTransfer
    {
        public async Task TransMoneyAsync(MoneyMailer sender, MoneyMailer target, CurrencySet acc, TransferType type, PostgresContext _context)
        {
            var trans = new MoneyTransfer
            {
                Person = sender,
                Target = target,
                Account = acc,
                Type = (int)type
            };

            _context.Add(trans);
            await _context.SaveChangesAsync();
        }

        public async Task TransMoneyAsync(Person sender, MoneyMailer target, CurrencySet acc, TransferType type,PostgresContext _context)
        {
            await TransMoneyAsync(sender.Mailer, target, acc, type, _context);
        }

        public async Task TransMoneyAsync(Person sender, Person target, CurrencySet acc, TransferType type,PostgresContext _context)
        {
            await TransMoneyAsync(sender.Mailer, target.Mailer, acc, type, _context);
        }
    }

    public enum TransferType
    {
        Message,
        Subscription,
        Like,
        Comment,
        Trans
    }
}
