using System;
using Microsoft.AspNetCore.Mvc;
using CZ.TUL.PWA.Messenger.Server.Model;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CZ.TUL.PWA.Messenger.Server.Config;
using Microsoft.Extensions.Configuration;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Route("api/auth")]
	public class AuthController : Controller
    {
        public IConfiguration Configuration { get; }

        public AuthController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] User user) 
        {
            if(user == null) 
            {
                return BadRequest("User not given");
            }

            if(user.UserName == "admin")
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

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                return Ok(new { Token = tokenString });
            }
            else
            {
                return Unauthorized();
            }
        }

        
    }
}
