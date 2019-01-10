using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyShareTermApp.Models
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

    public partial class CurrencySet
    {
        public Tuple<Char, decimal> GetOne() {
            if (!((Euro != 0 ^ Ruble != 0 ^ Dollar != 0) && !(Euro == 0 && Dollar == 0 && Ruble == 0)))
                throw new ArgumentException(); // только один из них должен быть равен 0

            if (Euro > 0)
                return Tuple.Create((char)Currency.Euro, Euro);

            if (Ruble > 0)
                return Tuple.Create((char)Currency.Ruble, Ruble);

            return Tuple.Create((char)Currency.Dollar, Dollar);
        }

        public bool CheckPositive()
        {
            return Euro >= 0 && Ruble >= 0 && Dollar >= 0;
        }

        public void Plus(CurrencySet other)
        {
            Euro += other.Euro;
            Ruble += other.Ruble;
            Dollar += other.Dollar;
        }

        public bool Minus(CurrencySet other)
        {
            Euro -= other.Euro;
            Ruble -= other.Ruble;
            Dollar -= other.Dollar;

            if (!CheckPositive())
            {
                Plus(other);
                return false;
            }

            return true;
        }
    }

    public enum Currency
    {
        Euro = '€',
        Ruble = '₽',
        Dollar = '$'
    }
}
