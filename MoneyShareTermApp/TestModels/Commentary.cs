using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyShareTermApp.TestModels
{
    [Table("commentary")]
    public partial class Commentary
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("post_id")]
        public int PostId { get; set; }
        [Column("mailer_id")]
        public int MailerId { get; set; }
        [Column("time")]
        public DateTime Time { get; set; }
        [Required]
        [Column("text")]
        public string Text { get; set; }

        [ForeignKey("MailerId")]
        [InverseProperty("Commentary")]
        public virtual MoneyMailer Mailer { get; set; }
        [ForeignKey("PostId")]
        [InverseProperty("Commentary")]
        public virtual Post Post { get; set; }
    }
}
