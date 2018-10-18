using System;
using CZ.TUL.PWA.Messenger.Server.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace CZ.TUL.PWA.Messenger.Server.Tests.Utilities
{
    public static class TestDataSeeding
    {
        public static void SeedTestUser(MessengerContext db) 
        {
            var user = new User
            {
                Name = "testuser",
                UserName = "testuser",
                NormalizedUserName = "TESTUSER"
            };

            var passwordHasher = new PasswordHasher<User>();
            var hash = passwordHasher.HashPassword(user, "password");

            user.PasswordHash = hash;

            var userStore = new UserStore<User>(db);
            userStore.CreateAsync(user);

            var user2 = new User
            {
                Name = "testuser2",
                UserName = "testuser2",
                NormalizedUserName = "TESTUSER2"
            };

            var passwordHasher2 = new PasswordHasher<User>();
            var hash2 = passwordHasher.HashPassword(user, "password");

            user2.PasswordHash = hash2;

            var userStore2 = new UserStore<User>(db);
            userStore.CreateAsync(user2);

            db.SaveChanges();
        }
    }
}
