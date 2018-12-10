using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CZ.TUL.PWA.Messenger.Server.Extensions;
using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.Services;
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using CZ.TUL.PWA.Messenger.Server.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ConversationsController : Controller
    {
        private readonly MessengerContext messengerContext;
        private readonly ITokenService tokenService;
        private readonly IConversationService conversationService;
        private readonly ILogger<ConversationsController> logger;

        public ConversationsController(
            MessengerContext messengerContext,
            IConversationService conversationService,
            ITokenService tokenService,
            ILogger<ConversationsController> logger)
        {
            this.messengerContext = messengerContext;
            this.conversationService = conversationService;
            this.tokenService = tokenService;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<ConversationViewModel> Post([FromBody] ConversationViewModel conversationViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                // TODO
                //return this.BadRequest(this.ModelState);
            }

            var conversation = new Conversation()
            {
                Name = conversationViewModel.Name
            };

            var result = await this.messengerContext.AddAsync(conversation);
            await this.messengerContext.SaveChangesAsync();

            return result.Entity.ToViewModel();
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

        [HttpGet("{id}")]
        public async Task<ConversationViewModel> Get([FromRoute] int id)
        {
            string userId = (await this.tokenService.GetCurrentUser(this.User)).Id;

            return await this.messengerContext.Conversations
                                    .Include(x => x.UserConversations)
                                    .Select(x => new ConversationViewModel
                                    {
                                        ConversationId = x.ConversationId,
                                        Name = x.Name,
                                        Addressees = x.UserConversations.Select(y => y.User.ToViewModel())
                                    })
                                    .SingleOrDefaultAsync(x => x.ConversationId == id
                                                          && x.Addressees.Any(y => y.Id == userId));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage([FromRoute] int id, [FromBody] ConversationViewModel conversation)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (id != conversation.ConversationId)
            {
                return this.BadRequest("Conversation ids are not the same.");
            }

            string userId = (await this.tokenService.GetCurrentUser(this.User)).Id;
            if (this.messengerContext.UserConversations.Any(x => x.UserId == userId && x.ConversationId == id) == false)
            {
                return this.BadRequest("Conversation not belongs to user");
            }

            Conversation conversationUpdated = new Conversation
            {
                ConversationId = conversation.ConversationId,
                Name = conversation.Name
            };

            this.messengerContext.Entry(conversationUpdated).State = EntityState.Modified;

            try
            {
                await this.messengerContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                this.logger.LogError("Unable to update Conversation", e);

                throw;
            }

            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            string userId = (await this.tokenService.GetCurrentUser(this.User)).Id;
            if (this.messengerContext.UserConversations.Any(x => x.UserId == userId && x.ConversationId == id) == false)
            {
                return this.BadRequest("Conversation not belongs to user");
            }

            Conversation conversation = await this.messengerContext.Conversations.FindAsync(id);

            this.messengerContext.Conversations.Remove(conversation);
            await this.messengerContext.SaveChangesAsync();

            return this.Ok();
        }
    }
}
