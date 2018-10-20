using System;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Text;
using Xunit.Sdk;
using CZ.TUL.PWA.Messenger.Server.Model;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

namespace CZ.TUL.PWA.Messenger.Server.Tests.Controllers
{
    public class AuthControllerIntegrationTest
        : IClassFixture<MessengerWebApplicationFactory<Startup>>
    {
        private readonly MessengerWebApplicationFactory<Startup> _factory;

        public AuthControllerIntegrationTest(MessengerWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Login_ShouldSuccess() 
        {
            var client = _factory.CreateClient();

            var data = new
            {
                UserName = "testuser",
                Password = "password"
            };

            var content = new StringContent(JsonConvert.SerializeObject(data).ToString(),
                                            Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/auth/login", content);

            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Refresh_ShouldSuccess()
        {
            var client = _factory.CreateClient();

            MessengerContext context = this._factory.Server.Host.Services.GetService(typeof(MessengerContext)) as MessengerContext;

            #region Login

            var data = new
            {
                UserName = "testuser",
                Password = "password"
            };

            var content = new StringContent(JsonConvert.SerializeObject(data).ToString(),
                                            Encoding.UTF8, "application/json");

            var loginResponse = await client.PostAsync("/api/auth/login", content);
            loginResponse.EnsureSuccessStatusCode();

            #endregion

            var stringLoginResponse = await loginResponse.Content.ReadAsStringAsync();
            var loginJsonResult = new JsonObject(stringLoginResponse);

            var refreshContent = new StringContent(loginJsonResult.Json, Encoding.UTF8, "application/json");
            var refreshResponse = await client.PostAsync("/api/auth/login", content);
            refreshResponse.EnsureSuccessStatusCode();

            var stringRefreshResponse = await refreshResponse.Content.ReadAsStringAsync();
            var refreshJsonResult = JObject.Parse(stringRefreshResponse);
            var refreshToken = refreshJsonResult.Value<string>("refreshToken");

            User user = context.Users.Single(x => x.UserName == "testuser");

            Assert.True(context.RefreshTokens.Any(x => x.Token == refreshToken
                                                  && x.UserId == user.Id));
        }
    }
}
