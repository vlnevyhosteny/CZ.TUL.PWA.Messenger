using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.Tests.Utilities;
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CZ.TUL.PWA.Messenger.Server.Tests.Controllers
{
    public class UsersControllerIntegrationTests
        : IClassFixture<MessengerWebApplicationFactory<Startup>>
    {
        private readonly MessengerWebApplicationFactory<Startup> _factory;

        public UsersControllerIntegrationTests(MessengerWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_ShouldSuccess()
        {
            var client = _factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                await AuthenticationUtilities.GetTestUserAccessTokenAsync(client));

            MessengerContext context = _factory.Server.Host.Services.GetService(typeof(MessengerContext)) 
                                               as MessengerContext;

            await context.Users.AddRangeAsync(GenerateUsers(5));

            var response = await client.GetAsync("/api/users");
            response.EnsureSuccessStatusCode();

            var users = response.Content.ReadAsAsync(typeof(IEnumerable<UserViewModel>)).Result as IEnumerable<UserViewModel>;

            int usersCount = await context.Users.CountAsync();

            Assert.NotNull(users);
            Assert.Equal(usersCount, users.Count());
        }

        [Fact]
        public async Task GetById_ShouldSuccess()
        {
            var client = _factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                await AuthenticationUtilities.GetTestUserAccessTokenAsync(client));

            MessengerContext context = _factory.Server.Host.Services.GetService(typeof(MessengerContext))
                                               as MessengerContext;

            await context.Users.AddRangeAsync(GenerateUsers(5));
            await context.SaveChangesAsync();

            var dbUser = await context.Users.Skip(2).FirstAsync();

            var response = await client.GetAsync($"/api/users/{dbUser.Id}");
            response.EnsureSuccessStatusCode();

            var user = response.Content.ReadAsAsync(typeof(UserViewModel)).Result as UserViewModel;

            Assert.NotNull(user);
            Assert.Equal(dbUser.UserName, user.UserName);
        }

        private IEnumerable<User> GenerateUsers(int count) 
        {
            var result = new User[count];

            for (int i = 0; i < count; i++) 
            {
                result[i] = new User
                {
                    UserName = $"gentestuser{i}"
                };
            }

            return result;
        }
    }
}
