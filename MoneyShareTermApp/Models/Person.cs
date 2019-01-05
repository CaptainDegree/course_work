using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyShareTermApp.Models
{
    [Table("person")]
    public partial class Person
    {
        public Person()
        {
            MessagePerson = new HashSet<Message>();
            MessageTarget = new HashSet<Message>();
            Post = new HashSet<Post>();
            SubscriptionPerson = new HashSet<Subscription>();
            SubscriptionSubscriber = new HashSet<Subscription>();
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("account_id")]
        public int AccountId { get; set; }
        [Column("message_price_id")]
        public int MessagePriceId { get; set; }
        [Column("comment_price_id")]
        public int CommentPriceId { get; set; }
        [Column("subscription_price_id")]
        public int SubscriptionPriceId { get; set; }
        [Column("photo_id")]
        public int? PhotoId { get; set; }
        [Column("mailer_id")]
        public int MailerId { get; set; }
        [Required]
        [Column("birthday", TypeName = "date")]
        public DateTime Birthday { get; set; }
        [Required]
        [Column("first_name")]
        public string FirstName { get; set; }
        [Required]
        [Column("middle_name")]
        public string MiddleName { get; set; }
        [Required]
        [Column("second_name")]
        public string SecondName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Column("password")]
        public string Password { get; set; }
        [Required]
        [Column("login")]
        public string Login { get; set; }
        [Required]
        [Column("phone_number")]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; }
        [Column("hidden")]
        public bool? Hidden { get; set; }

        [ForeignKey("AccountId")]
        [InverseProperty("PersonAccount")]
        public virtual CurrencySet Account { get; set; }
        [ForeignKey("CommentPriceId")]
        [InverseProperty("PersonCommentPrice")]
        public virtual CurrencySet CommentPrice { get; set; }
        [ForeignKey("MailerId")]
        [InverseProperty("Person")]
        public virtual MoneyMailer Mailer { get; set; }
        [ForeignKey("MessagePriceId")]
        [InverseProperty("PersonMessagePrice")]
        public virtual CurrencySet MessagePrice { get; set; }
        [ForeignKey("PhotoId")]
        [InverseProperty("Person")]
        public virtual File Photo { get; set; }
        [ForeignKey("SubscriptionPriceId")]
        [InverseProperty("PersonSubscriptionPrice")]
        public virtual CurrencySet SubscriptionPrice { get; set; }
        [InverseProperty("Person")]
        public virtual ICollection<Message> MessagePerson { get; set; }
        [InverseProperty("Target")]
        public virtual ICollection<Message> MessageTarget { get; set; }
        [InverseProperty("Person")]
        public virtual ICollection<Post> Post { get; set; }
        [InverseProperty("Person")]
        public virtual ICollection<Subscription> SubscriptionPerson { get; set; }
        [InverseProperty("Subscriber")]
        public virtual ICollection<Subscription> SubscriptionSubscriber { get; set; }
    }
}
