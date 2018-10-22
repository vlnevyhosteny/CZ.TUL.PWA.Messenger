using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CZ.TUL.PWA.Messenger.Server.Tests.Utilities
{
    public static class AuthenticationUtilities
    {
        public static async Task<string> GetTestUserAccessTokenAsync(HttpClient client) 
        {
            var data = new
            {
                UserName = "testuser",
                Password = "password"
            };

            var content = new StringContent(JsonConvert.SerializeObject(data).ToString(),
                                            Encoding.UTF8, "application/json");

            var loginResponse = await client.PostAsync("/api/auth/login", content);
            loginResponse.EnsureSuccessStatusCode();

            var stringLoginResponse = await loginResponse.Content.ReadAsStringAsync();
            var loginJsonResult = JObject.Parse(stringLoginResponse);

            return loginJsonResult.Value<string>("token");
        }

    }
}
