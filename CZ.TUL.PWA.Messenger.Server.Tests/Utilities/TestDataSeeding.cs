using System;
using CZ.TUL.PWA.Messenger.Server.Model;
using Microsoft.AspNetCore.Identity;
namespace CZ.TUL.PWA.Messenger.Server.Tests.Utilities
{
    public static class TestDataSeeding
    {
        public static void SeedTestUser(UserManager<User> userManager) 
        {
            userManager.CreateAsync(new User()
            {
                UserName = "testuser",
                Name = "test user"
            }, "testpass");
        }
    }
}
