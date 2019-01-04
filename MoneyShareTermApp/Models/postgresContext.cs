using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MoneyShareTermApp.Models
{
    public partial class postgresContext : DbContext
    {
        public postgresContext()
        {
        }

        public postgresContext(DbContextOptions<postgresContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Commentary> Commentary { get; set; }
        public virtual DbSet<CurrencySet> CurrencySet { get; set; }
        public virtual DbSet<File> File { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<MoneyMailer> MoneyMailer { get; set; }
        public virtual DbSet<MoneyTransfer> MoneyTransfer { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<Post> Post { get; set; }
        public virtual DbSet<Subscription> Subscription { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Host=localhost;Database=postgres;Username=renor;Password=Plokijuy1");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Commentary>(entity =>
            {
                entity.HasOne(d => d.Mailer)
                    .WithMany(p => p.Commentary)
                    .HasForeignKey(d => d.MailerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("commentary_mailer_id_fkey");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Commentary)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("commentary_post_id_fkey");
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.HasIndex(e => e.Link)
                    .HasName("file_link_key")
                    .IsUnique();

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.File)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("file_post_id_fkey");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasOne(d => d.Mailer)
                    .WithMany(p => p.Message)
                    .HasForeignKey(d => d.MailerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("message_mailer_id_fkey");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.MessagePerson)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("message_person_id_fkey");

                entity.HasOne(d => d.Target)
                    .WithMany(p => p.MessageTarget)
                    .HasForeignKey(d => d.TargetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("message_target_id_fkey");
            });

            modelBuilder.Entity<MoneyTransfer>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.MoneyTransfer)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("money_transfer_account_id_fkey");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.MoneyTransferPerson)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("money_transfer_person_id_fkey");

                entity.HasOne(d => d.Target)
                    .WithMany(p => p.MoneyTransferTarget)
                    .HasForeignKey(d => d.TargetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("money_transfer_target_id_fkey");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasIndex(e => e.Login)
                    .HasName("person_login_key")
                    .IsUnique();

                entity.Property(e => e.Hidden).HasDefaultValueSql("false");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.PersonAccount)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("person_account_id_fkey");

                entity.HasOne(d => d.CommentPrice)
                    .WithMany(p => p.PersonCommentPrice)
                    .HasForeignKey(d => d.CommentPriceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("person_comment_price_id_fkey");

                entity.HasOne(d => d.Mailer)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.MailerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("person_mailer_id_fkey");

                entity.HasOne(d => d.MessagePrice)
                    .WithMany(p => p.PersonMessagePrice)
                    .HasForeignKey(d => d.MessagePriceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("person_message_price_id_fkey");

                entity.HasOne(d => d.Photo)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.PhotoId)
                    .HasConstraintName("person_photo_id_fkey");

                entity.HasOne(d => d.SubscriptionPrice)
                    .WithMany(p => p.PersonSubscriptionPrice)
                    .HasForeignKey(d => d.SubscriptionPriceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("person_subscription_price_id_fkey");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasOne(d => d.Mailer)
                    .WithMany(p => p.Post)
                    .HasForeignKey(d => d.MailerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("post_mailer_id_fkey");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Post)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("post_person_id_fkey");

                entity.HasOne(d => d.PostNavigation)
                    .WithMany(p => p.InversePostNavigation)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("post_post_id_fkey");
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasOne(d => d.Mailer)
                    .WithMany(p => p.Subscription)
                    .HasForeignKey(d => d.MailerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("subscription_mailer_id_fkey");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.SubscriptionPerson)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("subscription_person_id_fkey");

                entity.HasOne(d => d.Subscriber)
                    .WithMany(p => p.SubscriptionSubscriber)
                    .HasForeignKey(d => d.SubscriberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("subscription_subscriber_id_fkey");
            });
        }
    }
}
