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
using CZ.TUL.PWA.Messenger.Server.Services;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly ITokenService tokenService;

        public AuthController(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }
        
        [HttpPost, Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserCredentialsViewModel givenUser)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var user = await this.tokenService.ValidateUser(givenUser.UserName, givenUser.Password);
            if (user == null)
            {
                // TODO
                return BadRequest();
            }

            string refreshToken = this.tokenService.GenerateRefreshToken();
            string token = this.tokenService.GenerateJwtToken(user.UserName);

            await this.tokenService.SetRefreshToken(user,
                                                    new RefreshToken()
                                                    {
                                                        Token = refreshToken,
                                                        UserId = user.Id
                                                    });

            return new OkObjectResult(new
            {   
                token,
                refreshToken
            });
        }

        [HttpPost, Route("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var userName = this.tokenService.GetUserNameFromJwtToken(model.Token);

            User user = await this.tokenService.ValidateRefreshToken(userName, model.RefreshToken);
            if(user == null) 
            {
                return BadRequest();
            }

            string newRefreshToken = this.tokenService.GenerateRefreshToken();
            string newJwtToken = this.tokenService.GenerateJwtToken(userName);

            await this.tokenService.SetRefreshToken(user,
                                        new RefreshToken()
                                        {
                                            Token = newRefreshToken,
                                            UserId = user.Id
                                        });

            return new ObjectResult(new
            {
                token = newJwtToken,
                refreshToken = newRefreshToken
            });
        }

    }
}
