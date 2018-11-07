using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly MessengerContext messengerContext;
        private readonly UserManager<User> userManager;
        private readonly ILogger logger;

        public UsersController(IConfiguration configuration, MessengerContext messengerContext, UserManager<User> userManager, ILogger logger)
        {
            this.configuration = configuration;
            this.messengerContext = messengerContext;
            this.userManager = userManager;
            this.logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserCredentialsViewModel userViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var user = new User()
            {
                UserName = userViewModel.UserName,
                Name = userViewModel.Name
            };

            var result = await this.userManager.CreateAsync(user, userViewModel.Password);

            if (!result.Succeeded)
            {
                this.logger.LogDebug("Unable to create user");

                return new BadRequestResult();
            }

            return new OkObjectResult("Account created");
        }

        [HttpGet]
        public async Task<IEnumerable<UserViewModel>> Get() => await this.messengerContext.Users
                             .Select(x => new UserViewModel { Id = x.Id, UserName = x.UserName, Name = x.Name })
                             .ToListAsync();

        [HttpGet("{id}")]
        public async Task<UserViewModel> Get(string id) => await this.messengerContext.Users
                                                                     .Select(x => new UserViewModel { Id = x.Id, UserName = x.UserName, Name = x.Name })
                                                                     .SingleOrDefaultAsync(x => x.Id == id);

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody]UserViewModel userViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var user = await this.userManager.FindByIdAsync(id);
            if (user == null)
            {
                return this.NotFound();
            }

            user.Name = userViewModel.Name;

            await this.userManager.UpdateAsync(user);

            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IAsyncResult> Delete(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Task.FromResult(this.NotFound());
            }

            await this.userManager.DeleteAsync(user);

            return Task.FromResult(this.NoContent());
        }
    }
}
