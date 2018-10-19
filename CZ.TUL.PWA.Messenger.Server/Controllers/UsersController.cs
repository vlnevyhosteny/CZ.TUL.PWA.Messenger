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
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly MessengerContext messengerContext;
        private readonly UserManager<User> userManager;

        public UsersController(IConfiguration configuration, MessengerContext messengerContext, UserManager<User> userManager)
        {
            this.configuration = configuration;
            this.messengerContext = messengerContext;
            this.userManager = userManager;
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
                // TODO
                return new BadRequestResult();
            }

            return new OkObjectResult("Account created");
        }

        [HttpGet]
        public async Task<IEnumerable<UserViewModel>> Get() => await this.messengerContext.Users
                             .Select(x => new UserViewModel(x.Id, x.UserName, x.Name))
                             .ToListAsync();

        [HttpGet("{id}")]
        public async Task<UserViewModel> Get(string id) => await this.messengerContext.Users
                                                                     .Select(x => new UserViewModel(x.Id, x.UserName, x.Name))
                                                                     .SingleOrDefaultAsync(x => x.Id == id);

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UserViewModel userViewModel)
        {
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
