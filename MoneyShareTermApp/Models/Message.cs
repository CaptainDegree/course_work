using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyShareTermApp.Models
{
    [Table("message")]
    public partial class Message
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("person_id")]
        public int PersonId { get; set; }
        [Column("target_id")]
        public int TargetId { get; set; }
        [Column("mailer_id")]
        public int MailerId { get; set; }
        [Column("time", TypeName = "date")]
        public DateTime Time { get; set; }
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
}
