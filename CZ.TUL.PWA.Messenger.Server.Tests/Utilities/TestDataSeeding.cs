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

            db.SaveChanges();
        }
    }
}
