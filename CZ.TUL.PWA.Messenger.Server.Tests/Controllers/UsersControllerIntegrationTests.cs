using System;
using Xunit;
using CZ.TUL.PWA.Messenger.Server.Model;
using System.Threading.Tasks;
using System.Net.Http;
using CZ.TUL.PWA.Messenger.Server.ViewModels;

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

            MessengerContext context = _factory.Server.Host.Services.GetService(typeof(MessengerContext)) 
                                               as MessengerContext;

            var response = await client.GetAsync("/api/users");
            response.EnsureSuccessStatusCode();

            var users = response.Content.ReadAsAsync(typeof(UserViewModel)).Result as UserViewModel;

            Assert.NotNull(users);
        }
    }
}
