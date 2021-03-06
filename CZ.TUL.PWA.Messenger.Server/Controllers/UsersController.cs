﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CZ.TUL.PWA.Messenger.Server.Extensions;
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
        private readonly ILogger<UsersController> logger;

        public UsersController(IConfiguration configuration, MessengerContext messengerContext, UserManager<User> userManager, ILogger<UsersController> logger)
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
                this.logger.LogDebug("Unable to create user", result.Errors.ToResponseString());

                return new BadRequestObjectResult(result.Errors.ToResponseString());
            }

            return new OkResult();
        }

        [HttpGet]
        public async Task<IEnumerable<UserViewModel>> Get([FromQuery] int limit = 100, [FromQuery]int offset = 0)
            => await this.messengerContext.Users
                            .Skip(offset)
                            .Take(limit)
                            .Select(x => new UserViewModel { Id = x.Id, UserName = x.UserName, Name = x.Name })
                            .ToListAsync();

        [HttpGet("UserNameContains/{contains}")]
        public async Task<IEnumerable<UserViewModel>> GetUserNameContains(string contains, [FromQuery] int limit = 100, [FromQuery]int offset = 0)
        {
            return await this.messengerContext.Users
                                       .Where(x => x.UserName.Contains(contains))
                                       .Skip(offset)
                                       .Take(limit)
                                       .Select(x => new UserViewModel { Id = x.Id, UserName = x.UserName, Name = x.Name })
                                       .ToListAsync();
        }

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
