using System;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Text;

namespace CZ.TUL.PWA.Messenger.Server.Tests.Controllers
{
    public class AuthControllerIntegrationTest
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public AuthControllerIntegrationTest(WebApplicationFactory<Startup> webApplicationFactory)
        {
            _factory = webApplicationFactory;
        }

        [Fact]
        public async Task Login_ShouldSuccess() 
        {
            var client = _factory.CreateClient();

            var data = new
            {
                UserName = "Maskicz",
                Password = "SuperStrongpasswd"
            };

            var content = new StringContent(JsonConvert.SerializeObject(data).ToString(),
                                            Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/auth/login", content);

            response.EnsureSuccessStatusCode();
        }
    }
}
