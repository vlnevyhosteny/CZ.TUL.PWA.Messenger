using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace CZ.TUL.PWA.Messenger.Server.Model
{
    public class MessengerContext : DbContext
    {
        public DbSet<User> Users
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

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<UserConversation>()
                        .HasKey(uc => new { uc.ConversationId, uc.UserId });

            modelBuilder.Entity<UserConversation>()
                        .HasOne(uc => uc.User)
                        .WithMany(u => u.UserConversations)
                        .HasForeignKey(uc => uc.UserId);
        }
    }
}
