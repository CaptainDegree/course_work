using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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
        public static void TransMoney(MoneyMailer sender, MoneyMailer target, CurrencySet acc, TransferType type, PostgresContext _context)
        {
            var trans = new MoneyTransfer
            {
                Person = sender,
                Target = target,
                Account = acc,
                Type = (int)type
            };

            // получить получателей, чтобы изменить аккаунты 
            var perSender = _context.Person
                .Include(p => p.Account)
                .SingleOrDefault(p => p.MailerId == sender.Id);

            Person perTarget;

            switch (type)
            {
                case TransferType.Comment:
                    perTarget = _context.Commentary
                        .Include(p => p.Mailer)
                        .Include(p => p.Post)
                            .ThenInclude(p => p.Person)
                                .ThenInclude(p => p.Account)
                        .SingleOrDefault(p => p.MailerId == target.Id)
                        .Post
                        .Person;
                    break;
                case TransferType.Like:
                    perTarget = _context.Post
                        .Include(p => p.Mailer)
                        .Include(p => p.Person)
                            .ThenInclude(p => p.Account)
                        .SingleOrDefault(p => p.MailerId == target.Id)
                        .Person;
                    break;
                case TransferType.Message:
                    var k = _context.Message
                        .Include(m => m.Mailer)
                        .Include(m => m.Target)
                            .ThenInclude(t => t.Account)
                        .SingleOrDefault(m => m.MailerId == target.Id);
                    perTarget = k.Target;
                    break;
                case TransferType.Trans:
                case TransferType.Subscription:
                    perTarget = _context.Person
                        .Include(p => p.Account)
                        .SingleOrDefault(p => p.MailerId == target.Id);
                    break;

                default:
                    throw new ArgumentException();
            }

            perTarget.Account.Plus(acc);
            if (!(perSender.Account.Minus(acc) && acc.CheckPositive()))
                throw new ArgumentException(); 

            _context.Add(trans);

            _context.SaveChanges();
            //_context.CurrencySet.Attach(perSender.Account);
            //_context.CurrencySet.Attach(perTarget.Account);

            //var entryUser = _context.Entry(perSender.Account);
            //var entryTarget = _context.Entry(perTarget.Account);

            //entryUser.IsModified = true;
            //entryTarget.Property(e => e.Dollar).IsModified = true;

        }

        public static void TransMoney(Person sender, MoneyMailer target, CurrencySet acc, TransferType type, PostgresContext _context)
        {
            TransMoney(sender.Mailer, target, acc, type, _context);
        }

        public static void TransMoneyAsync(Person sender, Person target, CurrencySet acc, TransferType type, PostgresContext _context)
        {
            TransMoney(sender.Mailer, target.Mailer, acc, type, _context);
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
