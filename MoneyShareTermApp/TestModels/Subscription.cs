using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyShareTermApp.TestModels
{
    [Table("subscription")]
    public partial class Subscription
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("person_id")]
        public int PersonId { get; set; }
        [Column("subscriber_id")]
        public int SubscriberId { get; set; }
        [Column("mailer_id")]
        public int MailerId { get; set; }

        [ForeignKey("MailerId")]
        [InverseProperty("Subscription")]
        public virtual MoneyMailer Mailer { get; set; }
        [ForeignKey("PersonId")]
        [InverseProperty("SubscriptionPerson")]
        public virtual Person Person { get; set; }
        [ForeignKey("SubscriberId")]
        [InverseProperty("SubscriptionSubscriber")]
        public virtual Person Subscriber { get; set; }
    }
}
