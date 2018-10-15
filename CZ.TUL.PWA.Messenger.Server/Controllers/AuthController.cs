using System;
using Microsoft.AspNetCore.Mvc;
using CZ.TUL.PWA.Messenger.Server.Model;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        public IConfiguration Configuration { get; }

        public MessengerContext MessengerContext
        {
            get;
        }

        public AuthController(IConfiguration configuration, MessengerContext messengerContext)
        {
            this.Configuration = configuration;
            this.MessengerContext = messengerContext;
        }

        [HttpPost, Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] GivenUser givenUser)
        {
            if (givenUser == null)
            {
                return BadRequest("User not given");
            }
                                                      
            return Unauthorized();
        }

        string ComposeJwtTokenString()
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AuthSecret:SecurityKey"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                issuer: Configuration["Auth:Issuer"],
                audience: Configuration["Auth:Audience"],
                claims: new List<Claim>(),
                expires: DateTime.Now.AddMinutes(Int32.Parse(Configuration["Auth:TokenExpiration"])),
                signingCredentials: signinCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }

        public class GivenUser 
        {
            public string UserName
            {
                get;
                set;
            }

            public string Password
            {
                get;
                set;
            }
        }
    }
}
