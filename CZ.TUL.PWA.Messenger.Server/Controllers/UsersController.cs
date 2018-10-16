using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly MessengerContext _messengerContext;
        private readonly UserManager<User> _userManager;

        public UsersController(IConfiguration configuration, MessengerContext messengerContext, UserManager<User> userManager)
        {
            this._configuration = configuration;
            this._messengerContext = messengerContext;
            this._userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User()
            {
                UserName = userViewModel.UserName,
                Name = userViewModel.Name
            };

            var result = await _userManager.CreateAsync(user, userViewModel.Password);

            if (!result.Succeeded)
            {
                // TODO
                return new BadRequestResult();
            }

            return new OkObjectResult("Account created");
        }
    }
}
