using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CZ.TUL.PWA.Messenger.Server.Model
{
    public class MessengerContext : IdentityDbContext<User>
    {
        public MessengerContext(DbContextOptions<MessengerContext> options)
            : base(options)
        { }

        public DbSet<User> MessengerUsers
        {
            get;
            set;
        }

        public DbSet<Conversation> Conversations
        {
            get;
            set;
        }

        public DbSet<Message> Messages
        {
            get;
            set;
        }

        protected override void OnModelCreating(ModelBuilder builder) 
        {
            SpecifyMySqlIndexLengthSpecification(builder);

            base.OnModelCreating(builder);

            builder.Entity<UserConversation>()
                   .HasKey(uc => new { uc.ConversationId, uc.UserId });

            builder.Entity<UserConversation>()
                   .HasOne(uc => uc.User)
                   .WithMany(u => u.UserConversations)
                   .HasForeignKey(uc => uc.UserId);

            builder.Entity<UserConversation>()
                   .HasOne(uc => uc.Conversation)
                   .WithMany(u => u.UserConversations)
                   .HasForeignKey(uc => uc.ConversationId);
        }

        private void SpecifyMySqlIndexLengthSpecification(ModelBuilder builder)
        {
            builder.Entity<User>().Property(m => m.UserName).HasMaxLength(127);
            builder.Entity<User>().Property(m => m.NormalizedUserName).HasMaxLength(127);
            builder.Entity<User>().Property(m => m.Id).HasMaxLength(127);
            builder.Entity<User>().Property(m => m.Name).HasMaxLength(127);
        }
    }
}
