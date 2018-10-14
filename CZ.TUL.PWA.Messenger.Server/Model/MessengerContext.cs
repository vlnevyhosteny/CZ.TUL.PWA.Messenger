﻿using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace CZ.TUL.PWA.Messenger.Server.Model
{
    public class MessengerContext : DbContext
    {
        public MessengerContext(DbContextOptions<MessengerContext> options)
            : base(options)
        { }

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

            modelBuilder.Entity<UserConversation>()
                        .HasOne(uc => uc.Conversation)
                        .WithMany(u => u.UserConversations)
                        .HasForeignKey(uc => uc.ConversationId);

            modelBuilder.Entity<User>()
                        .Property(p => p.UserId)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                        .HasIndex(u => u.UserName)
                        .IsUnique();

            modelBuilder.Entity<User>().HasData(
                new User() { UserId = 1, UserName = "admin" }
            );
        }
    }
}
