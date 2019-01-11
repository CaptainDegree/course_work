using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyShareTermApp.Models
{
    [Table("message")]
    public partial class Message : IComparable<Message>
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("person_id")]
        public int PersonId { get; set; }
        [Column("target_id")]
        public int TargetId { get; set; }
        [Column("mailer_id")]
        public int MailerId { get; set; }
        [Required]
        [Column("text")]
        public string Text { get; set; }

        [ForeignKey("MailerId")]
        [InverseProperty("Message")]
        public virtual MoneyMailer Mailer { get; set; }
        [ForeignKey("PersonId")]
        [InverseProperty("MessagePerson")]
        public virtual Person Person { get; set; }
        [ForeignKey("TargetId")]
        [InverseProperty("MessageTarget")]
        public virtual Person Target { get; set; }
    }

    public partial class Message : IComparable<Message>
    {
        public static readonly string MsgTransfer = "Перевод: ";
        public static readonly string SubTransfer = "Оплата подписки: ";
        public static readonly string LikeTransfer = "Пост оценили: ";
        public static readonly string SubSet = "На вас подписались";
        public static readonly string SubRemoved = "От вас отписались";

        public int CompareTo(Message other)
        {
            if (other == null)
                return 1;

            return Id.CompareTo(other.Id);
        }
    }
}
