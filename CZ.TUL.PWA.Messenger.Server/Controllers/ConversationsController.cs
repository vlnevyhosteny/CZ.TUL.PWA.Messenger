using CZ.TUL.PWA.Messenger.Server.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using CZ.TUL.PWA.Messenger.Server.Services;
using System.Threading.Tasks;
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CZ.TUL.PWA.Messenger.Server.Extensions;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ConversationsController : Controller
    {
        private readonly MessengerContext messengerContext;
        private readonly ITokenService tokenService;
        private readonly IConversationService conversationService;

        public ConversationsController(MessengerContext messengerContext, IConversationService conversationService, ITokenService tokenService)
        {
            this.messengerContext = messengerContext;
            this.conversationService = conversationService;
            this.tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ConversationViewModel conversationViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var conversation = new Conversation()
            {
                Name = conversationViewModel.Name
            };

            var result = await this.messengerContext.AddAsync(conversation);

            return new OkObjectResult(result);
        }

        [HttpGet]
        public async Task<IEnumerable<ConversationViewModel>> Get()
        {
            string userId = (await this.tokenService.GetCurrentUser(this.User)).Id;

            return await this.messengerContext.Conversations
                             .Include(x => x.UserConversations)
                             .Where(x => x.UserConversations
                                    .Any(y => y.UserId == userId))
                             .Select(x => new ConversationViewModel
                             {
                                 ConversationId = x.ConversationId,
                                 Name = x.Name,
                                 Addressees = x.UserConversations.Select(y => y.User.ToViewModel())
                             }).ToListAsync();
        }

        //[HttpGet("{id}")]
        //public async Task<UserViewModel> Get(string id) => await this.messengerContext.Users
        //                                                             .Select(x => new UserViewModel { Id = x.Id, UserName = x.UserName, Name = x.Name })
        //                                                             .SingleOrDefaultAsync(x => x.Id == id);

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(string id, [FromBody]UserViewModel userViewModel)
        //{
        //    if (!this.ModelState.IsValid)
        //    {
        //        return this.BadRequest();
        //    }

        //    var user = await this.userManager.FindByIdAsync(id);
        //    if (user == null)
        //    {
        //        return this.NotFound();
        //    }

        //    user.Name = userViewModel.Name;

        //    await this.userManager.UpdateAsync(user);

        //    return this.NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<IAsyncResult> Delete(string id)
        //{
        //    var user = await this.userManager.FindByIdAsync(id);
        //    if (user == null)
        //    {
        //        return Task.FromResult(this.NotFound());
        //    }

        //    await this.userManager.DeleteAsync(user);

        //    return Task.FromResult(this.NoContent());
        //}
    }
}
