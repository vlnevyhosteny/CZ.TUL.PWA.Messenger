// <copyright file="TokenService.cs" company="TUL">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CZ.TUL.PWA.Messenger.Server.Services
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using CZ.TUL.PWA.Messenger.Server.Model;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Logging;

    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<User> userManager;
        private readonly MessengerContext context;
        private readonly ILogger<TokenService> logger;

        public TokenService(
                            IConfiguration configuration,
                            UserManager<User> userManager,
                            MessengerContext context,
                            ILogger<TokenService> logger)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.context = context;
            this.logger = logger;
        }

        public string GenerateJwtToken(string userName)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["AuthSecret:SecurityKey"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims =
            {
                new Claim(ClaimTypes.Name, userName)
            };

            var tokenOptions = new JwtSecurityToken(
                issuer: this.configuration["Auth:Issuer"],
                audience: this.configuration["Auth:Audience"],
                claims: claims,
                expires: DateTime.Now.AddSeconds(int.Parse(this.configuration["Auth:TokenExpiration"])), // Change to minutes
                signingCredentials: signinCredentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (RandomNumberGenerator generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
        }

        public Task<User> GetCurrentUser(ClaimsPrincipal claimsPrincipal)
        {
            return this.userManager.FindByNameAsync(claimsPrincipal.Identity.Name);
        }

        public Task<RefreshToken> GetRefreshToken(User user) => this.context.RefreshTokens.SingleOrDefaultAsync(x => x.UserId == user.Id);

        public string GetUserNameFromJwtToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["AuthSecret:SecurityKey"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken)
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                this.logger.LogInformation("Invalid access token");

                throw new SecurityTokenException("Invalid token");
            }

            return principal.Claims.Single(x => x.Type == ClaimTypes.Name).Value;
        }

        public async Task RevokeRefreshToken(User user)
        {
            var existing = await this.context.RefreshTokens.SingleOrDefaultAsync(x => x.UserId == user.Id);
            if (existing == null)
            {
                return;
            }

            existing.Revoked = true;

            await this.context.SaveChangesAsync();
        }

        public async Task SetRefreshToken(User user, RefreshToken refreshToken)
        {
            refreshToken.Expires = DateTime.Now.AddMinutes(int.Parse(this.configuration["Auth:RefreshTokenExpiration"]));

            var existing = await this.context.RefreshTokens.SingleOrDefaultAsync(x => x.UserId == user.Id);
            if (existing != null)
            {
                this.context.Remove(existing);
                await this.context.SaveChangesAsync();
            }

            await this.context.RefreshTokens.AddAsync(refreshToken);
            await this.context.SaveChangesAsync();
        }

        public async Task<User> ValidateRefreshToken(string userName, string refreshToken)
        {
            var userToVerify = await this.userManager.FindByNameAsync(userName);
            if (userToVerify == null)
            {
                return null;
            }

            var savedRefreshToken = await this.GetRefreshToken(userToVerify);
            if (savedRefreshToken == null || savedRefreshToken.Token != refreshToken
                || savedRefreshToken.Revoked || savedRefreshToken.Expires <= DateTime.Now)
            {
                return null;
            }

            return userToVerify;
        }

        public async Task<User> ValidateUser(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return await Task.FromResult<User>(null);
            }

            var userToVerify = await this.userManager.FindByNameAsync(userName);
            if (userToVerify == null)
            {
                return await Task.FromResult<User>(null);
            }

            if (await this.userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(userToVerify);
            }

            return await Task.FromResult<User>(null);
        }
    }
}
