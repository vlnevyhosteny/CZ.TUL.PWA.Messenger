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
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<User> userManager;

        public AuthController(IConfiguration configuration, UserManager<User> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserCredentialsViewModel givenUser)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.BadRequest(this.ModelState);
            }

            var identity = await this.GetClaimsIdentity(givenUser.UserName, givenUser.Password);
            if (identity == null)
            {
                // TODO
                return this.BadRequest();
            }

            return new OkObjectResult(this.ComposeJwtTokenString());
        }

        private async Task<User> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return await Task.FromResult<User>(null);
            }

            // get the user to verifty
            var userToVerify = await this.userManager.FindByNameAsync(userName);
            if (userToVerify == null)
            {
                return await Task.FromResult<User>(null);
            }

            // check the credentials
            if (await this.userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(userToVerify);
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<User>(null);
        }

        private string ComposeJwtTokenString()
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["AuthSecret:SecurityKey"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                issuer: this.configuration["Auth:Issuer"],
                audience: this.configuration["Auth:Audience"],
                claims: new List<Claim>(),
                expires: DateTime.Now.AddMinutes(int.Parse(this.configuration["Auth:TokenExpiration"])),
                signingCredentials: signinCredentials);

            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }
    }
}
