using System.Threading.Tasks;
using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.Services;
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly ITokenService tokenService;
        private readonly ILogger logger;

        public AuthController(ITokenService tokenService, ILogger logger)
        {
            this.tokenService = tokenService;
            this.logger = logger;
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
                this.logger.LogInformation("Invalid login attempt");

                return this.BadRequest();
            }

            string refreshTokenString = this.tokenService.GenerateRefreshToken();
            string token = this.tokenService.GenerateJwtToken(user.UserName);

            RefreshToken refreshToken = new RefreshToken()
            {
                Token = refreshTokenString,
                UserId = user.Id
            };
            await this.tokenService.SetRefreshToken(user, refreshToken);

            return new OkObjectResult(new
            {
                token,
                refreshToken = refreshTokenString
            });
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenViewModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.BadRequest(this.ModelState);
            }

            var userName = this.tokenService.GetUserNameFromJwtToken(model.Token);

            User user = await this.tokenService.ValidateRefreshToken(userName, model.RefreshToken);
            if (user == null)
            {
                return this.BadRequest();
            }

            string newRefreshToken = this.tokenService.GenerateRefreshToken();
            string newJwtToken = this.tokenService.GenerateJwtToken(userName);

            await this.tokenService.SetRefreshToken(
                user,
                refreshToken: new RefreshToken() { Token = newRefreshToken, UserId = user.Id });

            return new ObjectResult(new
            {
                token = newJwtToken,
                refreshToken = newRefreshToken
            });
        }
    }
}
