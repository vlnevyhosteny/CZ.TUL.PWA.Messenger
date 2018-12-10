using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.Tests.Utilities;
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;
using Microsoft.AspNetCore.Identity;

namespace CZ.TUL.PWA.Messenger.Server.Tests.Controllers
{
    public class UsersControllerIntegrationTests
        : IClassFixture<MessengerWebApplicationFactory<Startup>>
    {
        private readonly MessengerWebApplicationFactory<Startup> factory;

        public UsersControllerIntegrationTests(MessengerWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task Get_ShouldSuccess()
        {
            var client = this.factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await AuthenticationUtilities.GetTestUserAccessTokenAsync(client));

            MessengerContext context = this.factory.Server.Host.Services.GetService(typeof(MessengerContext))
                                               as MessengerContext;

            await context.Users.AddRangeAsync(this.GenerateUsers(5));

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
            var client = this.factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await AuthenticationUtilities.GetTestUserAccessTokenAsync(client));

            MessengerContext context = this.factory.Server.Host.Services.GetService(typeof(MessengerContext))
                                               as MessengerContext;

            await context.Users.AddRangeAsync(this.GenerateUsers(5));
            await context.SaveChangesAsync();

            var dbUser = await context.Users.Skip(2).FirstAsync();

            var response = await client.GetAsync($"/api/users/{dbUser.Id}");
            response.EnsureSuccessStatusCode();

            var user = response.Content.ReadAsAsync(typeof(UserViewModel)).Result as UserViewModel;

            Assert.NotNull(user);
            Assert.Equal(dbUser.UserName, user.UserName);
        }

        [Fact]
        public async Task GetUserNameContains_ShouldSuccess()
        {
            var client = this.factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await AuthenticationUtilities.GetTestUserAccessTokenAsync(client));

            MessengerContext context = this.factory.Server.Host.Services.GetService(typeof(MessengerContext))
                                               as MessengerContext;

            UserManager<User> userManager = this.factory.Server.Host.Services.GetService(typeof(UserManager<User>))
                                    as UserManager<User>;

            foreach (var item in this.GenerateUsers(5))
            {
                await userManager.CreateAsync(item);
            }

            var dbUser = await context.Users.Skip(2).FirstAsync();

            var searchString = dbUser.UserName.Substring(0, 2);

            var response = await client.GetAsync($"/api/users/UserNameContains/{searchString}");
            response.EnsureSuccessStatusCode();

            var users = response.Content.ReadAsAsync(typeof(IEnumerable<UserViewModel>)).Result as IEnumerable<UserViewModel>;

            Assert.True(users.Any(x => x.UserName == dbUser.UserName));
        }

        [Fact]
        public async Task PutUpdate_ShouldSuccess()
        {
            var client = this.factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await AuthenticationUtilities.GetTestUserAccessTokenAsync(client));

            MessengerContext context = this.factory.Server.Host.Services.GetService(typeof(MessengerContext))
                                               as MessengerContext;

            UserManager<User> userManager = this.factory.Server.Host.Services.GetService(typeof(UserManager<User>))
                                    as UserManager<User>;

            foreach (var item in this.GenerateUsers(5))
            {
                await userManager.CreateAsync(item);
            }

            var dbUser = await context.Users.Skip(2).FirstAsync();

            string newName = "New Name";

            var requestBody = new UserViewModel { Id = dbUser.Id, UserName = dbUser.UserName, Name = newName };
            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/users/{dbUser.Id}", requestContent);
            response.EnsureSuccessStatusCode();

            userManager = this.factory.Server.Host.Services.GetService(typeof(UserManager<User>))
                                    as UserManager<User>;

            var updatedUser = await userManager.FindByIdAsync(dbUser.Id);

            Assert.Equal(newName, updatedUser.Name);
        }

        [Fact]
        public async Task Post_ShouldSuccess()
        {
            var client = this.factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await AuthenticationUtilities.GetTestUserAccessTokenAsync(client));

            MessengerContext context = this.factory.Server.Host.Services.GetService(typeof(MessengerContext))
                                               as MessengerContext;

            User user = this.GenerateUsers(1).Single();

            var requestBody = new UserCredentialsViewModel { Password = "testpwd", UserName = user.UserName, Name = user.Name };
            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/users", requestContent);
            response.EnsureSuccessStatusCode();

            UserManager<User> userManager = this.factory.Server.Host.Services.GetService(typeof(UserManager<User>))
                                    as UserManager<User>;

            var insertedUser = await userManager.FindByNameAsync(user.UserName);

            Assert.NotNull(insertedUser);
        }

        [Fact]
        public async Task Delete_ShouldSuccess()
        {
            var client = this.factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await AuthenticationUtilities.GetTestUserAccessTokenAsync(client));

            MessengerContext context = this.factory.Server.Host.Services.GetService(typeof(MessengerContext))
                                               as MessengerContext;

            await context.Users.AddRangeAsync(this.GenerateUsers(5));
            await context.SaveChangesAsync();

            var dbUser = await context.Users.Skip(2).FirstAsync();
            int previousCount = await context.Users.CountAsync();

            var response = await client.DeleteAsync($"/api/users/{dbUser.Id}");
            response.EnsureSuccessStatusCode();

            int currentCount = await context.Users.CountAsync();

            Assert.NotEqual(previousCount, currentCount);
            Assert.Equal(previousCount, currentCount + 1);
            Assert.False(await context.Users.AnyAsync(x => x.Id == dbUser.Id));
        }

        private IEnumerable<User> GenerateUsers(int count)
        {
            var result = new User[count];

            for (int i = 0; i < count; i++)
            {
                result[i] = new User
                {
                    UserName = $"gentestuser{i}",
                    Name = $"gettestusername{i}"
                };
            }

            return result;
        }
    }
}
