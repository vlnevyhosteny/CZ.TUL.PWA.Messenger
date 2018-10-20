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
using Microsoft.EntityFrameworkCore;
using CZ.TUL.PWA.Messenger.Server.ViewModels;

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
        public async Task Login_BadCredentials_ShouldFail()
        {
            var client = _factory.CreateClient();

            var data = new
            {
                UserName = "testuser",
                Password = "badpassword"
            };

            var content = new StringContent(JsonConvert.SerializeObject(data).ToString(),
                                            Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/auth/login", content);
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_NotValidModel_ShouldFail()
        {
            var client = _factory.CreateClient();

            var data = new
            {
                UserName = "testuser"
            };

            var content = new StringContent(JsonConvert.SerializeObject(data).ToString(),
                                            Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/auth/login", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
            var loginJsonResult = JObject.Parse(stringLoginResponse);
            
            var refreshTokenViewModel = new RefreshTokenViewModel()
            {
                RefreshToken = loginJsonResult.Value<string>("refreshToken"),
                Token = loginJsonResult.Value<string>("token")
            };

            var refreshContent = new StringContent(JsonConvert.SerializeObject(refreshTokenViewModel), Encoding.UTF8, "application/json");
            var refreshResponse = await client.PostAsync("/api/auth/refresh", refreshContent);
            refreshResponse.EnsureSuccessStatusCode();

            var stringRefreshResponse = await refreshResponse.Content.ReadAsStringAsync();
            var refreshJsonResult = JObject.Parse(stringRefreshResponse);
            var refreshToken = refreshJsonResult.Value<string>("refreshToken");

            User user = await context.Users.SingleAsync(x => x.UserName == "testuser");

            Assert.True(context.RefreshTokens.Any(x => x.Token == refreshToken
                                                  && x.UserId == user.Id));
        }

        [Fact]
        public async Task Refresh_ExpiredRefreshToken_ShouldSuccess()
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

            User user = await context.Users.SingleAsync(x => x.UserName == "testuser");
            RefreshToken refreshToken = await context.RefreshTokens.SingleAsync(x => x.UserId == user.Id);
            refreshToken.Expires = DateTime.Now.AddDays(-1);
            await context.SaveChangesAsync();

            var stringLoginResponse = await loginResponse.Content.ReadAsStringAsync();
            var loginJsonResult = JObject.Parse(stringLoginResponse);

            var refreshTokenViewModel = new RefreshTokenViewModel()
            {
                RefreshToken = loginJsonResult.Value<string>("refreshToken"),
                Token = loginJsonResult.Value<string>("token")
            };

            var refreshContent = new StringContent(JsonConvert.SerializeObject(refreshTokenViewModel), Encoding.UTF8, "application/json");
            var refreshResponse = await client.PostAsync("/api/auth/refresh", refreshContent);

            Assert.Equal(HttpStatusCode.BadRequest, refreshResponse.StatusCode);
        }

        [Fact]
        public async Task Refresh_RevokedRefreshToken_ShouldSuccess()
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

            User user = await context.Users.SingleAsync(x => x.UserName == "testuser");
            RefreshToken refreshToken = await context.RefreshTokens.SingleAsync(x => x.UserId == user.Id);
            refreshToken.Revoked = true;
            await context.SaveChangesAsync();

            var stringLoginResponse = await loginResponse.Content.ReadAsStringAsync();
            var loginJsonResult = JObject.Parse(stringLoginResponse);

            var refreshTokenViewModel = new RefreshTokenViewModel()
            {
                RefreshToken = loginJsonResult.Value<string>("refreshToken"),
                Token = loginJsonResult.Value<string>("token")
            };

            var refreshContent = new StringContent(JsonConvert.SerializeObject(refreshTokenViewModel), Encoding.UTF8, "application/json");
            var refreshResponse = await client.PostAsync("/api/auth/refresh", refreshContent);

            Assert.Equal(HttpStatusCode.BadRequest, refreshResponse.StatusCode);
        }

        [Fact]
        public async Task Refresh_BadRefreshTokenValue_ShouldSuccess()
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
            var loginJsonResult = JObject.Parse(stringLoginResponse);
            loginJsonResult["refreshToken"] = "badtoken";

            var refreshContent = new StringContent(loginJsonResult.ToString(), Encoding.UTF8, "application/json");
            var refreshResponse = await client.PostAsync("/api/auth/refresh", refreshContent);

            Assert.Equal(HttpStatusCode.BadRequest, refreshResponse.StatusCode);
        }
    }
}
