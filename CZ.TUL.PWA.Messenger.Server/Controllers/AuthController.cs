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
using System.Security.Cryptography;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        
        public AuthController(IConfiguration configuration, UserManager<User> userManager)
        {
            this._configuration = configuration;
            this._userManager = userManager;
        }
        
        [HttpPost, Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserCredentialsViewModel givenUser)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var identity = await GetClaimsIdentity(givenUser.UserName, givenUser.Password);
            if (identity == null)
            {
                // TODO
                return BadRequest();
            }

            string refreshToken = GenerateRefreshToken();
            string token = ComposeJwtTokenString(givenUser.UserName);

            await _userManager.SetAuthenticationTokenAsync(identity, "jwt", "refreshToken", refreshToken);

            return new OkObjectResult(new
            {
                token,
                refreshToken
            });
        }

        [HttpPost]
        public async Task<IActionResult> RefreshAsync(string token, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            var userName = principal.Identity.Name;

            var userToVerify = await _userManager.FindByNameAsync(userName);
            if (userToVerify == null)
            {
                return BadRequest();
            }

            var savedRefreshToken = await _userManager.GetAuthenticationTokenAsync(userToVerify, "jwt", "refreshToken");
            if (savedRefreshToken == null || savedRefreshToken != refreshToken)
            {
                return BadRequest();
            }

            var newJwtToken = ComposeJwtTokenString(userName);
            var newRefreshToken = GenerateRefreshToken();

            await _userManager.RemoveAuthenticationTokenAsync(userToVerify, "jwt", "refreshToken");
            await _userManager.SetAuthenticationTokenAsync(userToVerify, "jwt", "refreshToken", newRefreshToken);

            return new ObjectResult(new
            {
                token = newJwtToken,
                refreshToken = newRefreshToken
            });
        }

        private async Task SetRefreshToken(User user, RefreshToken refreshToken)
        {
            var existing = context.RefreshTokens.SingleOrDefaultAsync(x => x.UserId == user.Id);
            if(existing != null)
            {
                context.Delete(existing);
                context.SaveChanges();
            }

            context.RefreshTokens.Add(refreshToken);
            context.SaveChanges();
        }

        private async Task<RefreshToken> GetRefreshToken(User user)
        {
            return context.RefreshTokens.SingleOrDefaultAsync(x => x.UserId == user.Id);
        }

        private async Task RevokeRefreshToken(User user)
        {
            var existing = context.RefreshTokens.SingleOrDefaultAsync(x => x.UserId == user.Id);
            if (existing == null)
            {
                return;
            }

            existing.Revoked = true;

            context.SaveChanges();
        }

        private async Task<User> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return await Task.FromResult<User>(null);
            }

            // get the user to verifty
            var userToVerify = await _userManager.FindByNameAsync(userName);
            if (userToVerify == null)
            {
                return await Task.FromResult<User>(null);
            }

            // check the credentials
            if (await _userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(userToVerify);
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<User>(null);
        }

        string ComposeJwtTokenString(string userName)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSecret:SecurityKey"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, userName)
            };

            var tokeOptions = new JwtSecurityToken(
                issuer: _configuration["Auth:Issuer"],
                audience: _configuration["Auth:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Int32.Parse(_configuration["Auth:TokenExpiration"])),
                signingCredentials: signinCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }

        string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
        }

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("the server key used to sign the JWT token is here, use more than 16 chars")),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
