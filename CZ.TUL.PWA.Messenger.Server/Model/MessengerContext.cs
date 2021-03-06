﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CZ.TUL.PWA.Messenger.Server.Model
{
    public class MessengerContext : IdentityDbContext<User>
    {
        public MessengerContext(DbContextOptions<MessengerContext> options)
            : base(options)
        {
        }

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

        public DbSet<RefreshToken> RefreshTokens
        {
            get;
            set;
        }

        public DbSet<UserConversation> UserConversations
        {
            get;
            set;
        }

        public DbSet<HubConnection> HubConnections
        { 
            get;
            set;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            this.SpecifyMySqlIndexLengthSpecification(builder);

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

            builder.Entity<RefreshToken>()
                   .HasKey(t => t.UserId);

            builder.Entity<RefreshToken>()
                   .HasOne(t => t.User)
                   .WithOne()
                   .HasForeignKey<RefreshToken>(t => t.UserId);

            builder.Entity<Message>()
                   .Property(x => x.MessageId)
                   .ValueGeneratedOnAdd();

            builder.Entity<Conversation>()
                   .Property(x => x.ConversationId)
                   .ValueGeneratedOnAdd();

            // This is made for MySQL. TODO: should be generic
            builder.Entity<Message>()
                   .Property(x => x.DateSent)
                   .HasColumnType("TIMESTAMP");
            builder.Entity<Message>()
                   .Property(x => x.DateSent)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
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
