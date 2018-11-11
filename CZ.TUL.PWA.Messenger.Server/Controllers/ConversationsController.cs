using CZ.TUL.PWA.Messenger.Server.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using CZ.TUL.PWA.Messenger.Server.Services;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ConversationsController : Controller
    {
        private readonly MessengerContext messengerContext;
        private readonly UserManager<User> userManager;
        private readonly ConversationService conversationService;

        public ConversationsController(MessengerContext messengerContext, UserManager<User> userManager, ConversationService conversationService)
        {
            this.messengerContext = messengerContext;
            this.userManager = userManager;
            this.conversationService = conversationService;
        }

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
