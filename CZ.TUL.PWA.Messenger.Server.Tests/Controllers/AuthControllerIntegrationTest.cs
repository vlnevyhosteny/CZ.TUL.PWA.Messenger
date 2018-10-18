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
    }
}
