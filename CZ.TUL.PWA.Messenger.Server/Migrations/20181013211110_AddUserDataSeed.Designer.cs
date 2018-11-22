﻿// <auto-generated />
using System;
using CZ.TUL.PWA.Messenger.Server.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CZ.TUL.PWA.Messenger.Server.Migrations
{
    [DbContext(typeof(MessengerContext))]
    [Migration("20181013211110_AddUserDataSeed")]
    partial class AddUserDataSeed
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CZ.TUL.PWA.Messenger.Server.Model.Conversation", b =>
                {
                    b.Property<int>("ConversationId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.HasKey("ConversationId");

                    b.ToTable("Conversations");
                });

            modelBuilder.Entity("CZ.TUL.PWA.Messenger.Server.Model.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content");

                    b.Property<int?>("ConversationId");

                    b.Property<DateTime>("DateSent")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int?>("OwnerUserId");

                    b.HasKey("MessageId");

                    b.HasIndex("ConversationId");

                    b.HasIndex("OwnerUserId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("CZ.TUL.PWA.Messenger.Server.Model.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Password");

                    b.Property<string>("UserName");

                    b.HasKey("UserId");

                    b.ToTable("Users");

                    b.HasData(
                        new { UserId = 1, UserName = "admin" }
                    );
                });

            modelBuilder.Entity("CZ.TUL.PWA.Messenger.Server.Model.UserConversation", b =>
                {
                    b.Property<int>("ConversationId");

                    b.Property<int>("UserId");

                    b.Property<bool>("IsOwner");

                    b.HasKey("ConversationId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserConversation");
                });

            modelBuilder.Entity("CZ.TUL.PWA.Messenger.Server.Model.Message", b =>
                {
                    b.HasOne("CZ.TUL.PWA.Messenger.Server.Model.Conversation", "Conversation")
                        .WithMany("Messages")
                        .HasForeignKey("ConversationId");

                    b.HasOne("CZ.TUL.PWA.Messenger.Server.Model.User", "Owner")
                        .WithMany("Messages")
                        .HasForeignKey("OwnerUserId");
                });

            modelBuilder.Entity("CZ.TUL.PWA.Messenger.Server.Model.UserConversation", b =>
                {
                    b.HasOne("CZ.TUL.PWA.Messenger.Server.Model.Conversation", "Conversation")
                        .WithMany("UserConversations")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CZ.TUL.PWA.Messenger.Server.Model.User", "User")
                        .WithMany("UserConversations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
