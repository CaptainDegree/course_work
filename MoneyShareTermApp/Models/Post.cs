using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyShareTermApp.Models
{
    [Table("post")]
    public partial class Post
    {
        public Post()
        {
            Mailer = new MoneyMailer();
            Commentary = new HashSet<Commentary>();
            File = new HashSet<File>();
            InversePostNavigation = new HashSet<Post>();
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("person_id")]
        public int PersonId { get; set; }
        [Column("post_id")]
        public int? PostId { get; set; }
        [Column("mailer_id")]
        public int MailerId { get; set; }
        [Column("text")]
        public string Text { get; set; }

        [ForeignKey("MailerId")]
        [InverseProperty("Post")]
        public virtual MoneyMailer Mailer { get; set; }
        [ForeignKey("PersonId")]
        [InverseProperty("Post")]
        public virtual Person Person { get; set; }
        [ForeignKey("PostId")]
        [InverseProperty("InversePostNavigation")]
        public virtual Post PostNavigation { get; set; }
        [InverseProperty("Post")]
        public virtual ICollection<Commentary> Commentary { get; set; }
        [InverseProperty("Post")]
        public virtual ICollection<File> File { get; set; }
        [InverseProperty("PostNavigation")]
        public virtual ICollection<Post> InversePostNavigation { get; set; }
    }
}
