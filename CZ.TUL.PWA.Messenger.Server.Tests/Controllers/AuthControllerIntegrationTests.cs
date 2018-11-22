using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CZ.TUL.PWA.Messenger.Server.Tests.Controllers
{
    public class AuthControllerIntegrationTests
        : IClassFixture<MessengerWebApplicationFactory<Startup>>
    {
        private readonly MessengerWebApplicationFactory<Startup> factory;

        public AuthControllerIntegrationTests(MessengerWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task Login_ShouldSuccess()
        {
            HttpClient client = this.factory.CreateClient();

            var data = new
            {
                UserName = "testuser",
                Password = "password"
            };

            StringContent content = new StringContent(JsonConvert.SerializeObject(data).ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("/api/auth/login", content);

            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Login_BadCredentials_ShouldFail()
        {
            HttpClient client = this.factory.CreateClient();

            var data = new
            {
                UserName = "testuser",
                Password = "badpassword"
            };

            StringContent content = new StringContent(JsonConvert.SerializeObject(data).ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("/api/auth/login", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_NotValidModel_ShouldFail()
        {
            HttpClient client = this.factory.CreateClient();

            var data = new
            {
                UserName = "testuser"
            };

            StringContent content = new StringContent(JsonConvert.SerializeObject(data).ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("/api/auth/login", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Refresh_ShouldSuccess()
        {
            HttpClient client = this.factory.CreateClient();

            MessengerContext context = this.factory.Server.Host.Services.GetService(typeof(MessengerContext)) as MessengerContext;

            #region Login

            var data = new
            {
                UserName = "testuser",
                Password = "password"
            };

            StringContent content = new StringContent(JsonConvert.SerializeObject(data).ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage loginResponse = await client.PostAsync("/api/auth/login", content);
            loginResponse.EnsureSuccessStatusCode();

            #endregion

            string stringLoginResponse = await loginResponse.Content.ReadAsStringAsync();
            JObject loginJsonResult = JObject.Parse(stringLoginResponse);

            var refreshTokenViewModel = new RefreshTokenViewModel()
            {
                RefreshToken = loginJsonResult.Value<string>("refreshToken"),
                Token = loginJsonResult.Value<string>("token")
            };

            StringContent refreshContent = new StringContent(JsonConvert.SerializeObject(refreshTokenViewModel), Encoding.UTF8, "application/json");
            HttpResponseMessage refreshResponse = await client.PostAsync("/api/auth/refresh", refreshContent);
            refreshResponse.EnsureSuccessStatusCode();

            string stringRefreshResponse = await refreshResponse.Content.ReadAsStringAsync();
            JObject refreshJsonResult = JObject.Parse(stringRefreshResponse);
            string refreshToken = refreshJsonResult.Value<string>("refreshToken");

            User user = await context.Users.SingleAsync(x => x.UserName == "testuser");

            Assert.True(context.RefreshTokens.Any(x => x.Token == refreshToken
                                                  && x.UserId == user.Id));
        }

        [Fact]
        public async Task Refresh_ExpiredRefreshToken_ShouldSuccess()
        {
            HttpClient client = this.factory.CreateClient();

            MessengerContext context = this.factory.Server.Host.Services.GetService(typeof(MessengerContext)) as MessengerContext;

            #region Login

            var data = new
            {
                UserName = "testuser",
                Password = "password"
            };

            StringContent content = new StringContent(JsonConvert.SerializeObject(data).ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage loginResponse = await client.PostAsync("/api/auth/login", content);
            loginResponse.EnsureSuccessStatusCode();

            #endregion

            User user = await context.Users.SingleAsync(x => x.UserName == "testuser");
            RefreshToken refreshToken = await context.RefreshTokens.SingleAsync(x => x.UserId == user.Id);
            refreshToken.Expires = DateTime.Now.AddDays(-1);
            await context.SaveChangesAsync();

            string stringLoginResponse = await loginResponse.Content.ReadAsStringAsync();
            JObject loginJsonResult = JObject.Parse(stringLoginResponse);

            var refreshTokenViewModel = new RefreshTokenViewModel()
            {
                RefreshToken = loginJsonResult.Value<string>("refreshToken"),
                Token = loginJsonResult.Value<string>("token")
            };

            StringContent refreshContent = new StringContent(JsonConvert.SerializeObject(refreshTokenViewModel), Encoding.UTF8, "application/json");
            HttpResponseMessage refreshResponse = await client.PostAsync("/api/auth/refresh", refreshContent);

            Assert.Equal(HttpStatusCode.BadRequest, refreshResponse.StatusCode);
        }

        [Fact]
        public async Task Refresh_RevokedRefreshToken_ShouldSuccess()
        {
            var client = this.factory.CreateClient();

            MessengerContext context = this.factory.Server.Host.Services.GetService(typeof(MessengerContext)) as MessengerContext;

            #region Login

            var data = new
            {
                UserName = "testuser",
                Password = "password"
            };

            StringContent content = new StringContent(JsonConvert.SerializeObject(data).ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage loginResponse = await client.PostAsync("/api/auth/login", content);
            loginResponse.EnsureSuccessStatusCode();

            #endregion

            User user = await context.Users.SingleAsync(x => x.UserName == "testuser");
            RefreshToken refreshToken = await context.RefreshTokens.SingleAsync(x => x.UserId == user.Id);
            refreshToken.Revoked = true;
            await context.SaveChangesAsync();

            string stringLoginResponse = await loginResponse.Content.ReadAsStringAsync();
            JObject loginJsonResult = JObject.Parse(stringLoginResponse);

            var refreshTokenViewModel = new RefreshTokenViewModel()
            {
                RefreshToken = loginJsonResult.Value<string>("refreshToken"),
                Token = loginJsonResult.Value<string>("token")
            };

            StringContent refreshContent = new StringContent(JsonConvert.SerializeObject(refreshTokenViewModel), Encoding.UTF8, "application/json");
            HttpResponseMessage refreshResponse = await client.PostAsync("/api/auth/refresh", refreshContent);

            Assert.Equal(HttpStatusCode.BadRequest, refreshResponse.StatusCode);
        }

        [Fact]
        public async Task Refresh_BadRefreshTokenValue_ShouldSuccess()
        {
            HttpClient client = this.factory.CreateClient();

            MessengerContext context = this.factory.Server.Host.Services.GetService(typeof(MessengerContext)) as MessengerContext;

            #region Login

            var data = new
            {
                UserName = "testuser",
                Password = "password"
            };

            StringContent content = new StringContent(JsonConvert.SerializeObject(data).ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage loginResponse = await client.PostAsync("/api/auth/login", content);
            loginResponse.EnsureSuccessStatusCode();

            #endregion

            string stringLoginResponse = await loginResponse.Content.ReadAsStringAsync();
            JObject loginJsonResult = JObject.Parse(stringLoginResponse);
            loginJsonResult["refreshToken"] = "badtoken";

            StringContent refreshContent = new StringContent(loginJsonResult.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage refreshResponse = await client.PostAsync("/api/auth/refresh", refreshContent);

            Assert.Equal(HttpStatusCode.BadRequest, refreshResponse.StatusCode);
        }
    }
}
