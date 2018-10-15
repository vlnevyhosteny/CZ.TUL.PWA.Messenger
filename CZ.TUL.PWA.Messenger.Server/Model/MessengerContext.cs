using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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

            builder.Entity<User>()
                   .Property(p => p.Id)
                    .ValueGeneratedOnAdd();

            builder.Entity<User>()
                   .HasIndex(u => u.Name)
                   .IsUnique();

            //builder.Entity<MessengerUser>().HasData(
            //    new MessengerUser() { MessengerUserId = 1, Name = "admin" }
            //);
        }
    }
}
