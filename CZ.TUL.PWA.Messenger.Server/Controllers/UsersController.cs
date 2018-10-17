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
        public async Task<IActionResult> Post([FromBody] UserCredentialsViewModel userViewModel)
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

        [HttpGet]
        public async Task<IEnumerable<UserViewModel>> Get() => await this._messengerContext.Users
                             .Select(x => new UserViewModel(x.Id, x.UserName, x.Name))
                             .ToListAsync();

        [HttpGet("{id}")]
        public async Task<UserViewModel> Get(string id) => await this._messengerContext.Users
                                                                     .Select(x => new UserViewModel(x.Id, x.UserName, x.Name))
                                                                     .SingleOrDefaultAsync(x => x.Id == id);

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UserViewModel userViewModel) 
        {
            var user = await this._userManager.FindByIdAsync(id);
            if(user == null) 
            {
                return NotFound();
            }

            user.Name = userViewModel.Name;

            await this._userManager.UpdateAsync(user);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IAsyncResult> Delete(string id) 
        {
            var user = await this._userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Task.FromResult(NotFound());
            }

            await this._userManager.DeleteAsync(user);

            return Task.FromResult(NoContent());
        }
    }
}
