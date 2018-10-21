using System.Threading.Tasks;
using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.Services;
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserCredentialsViewModel givenUser)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.BadRequest(this.ModelState);
            }

            var user = await this.tokenService.ValidateUser(givenUser.UserName, givenUser.Password);
            if (user == null)
            {
                // TODO
                return this.BadRequest();
            }

            string refreshTokenString = this.tokenService.GenerateRefreshToken();
            string token = this.tokenService.GenerateJwtToken(user.UserName);

            RefreshToken refreshToken = new RefreshToken()
            {
                Token = refreshTokenString,
                UserId = user.Id
            };
            await this.tokenService.SetRefreshToken(user,
                                                    refreshToken);

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
