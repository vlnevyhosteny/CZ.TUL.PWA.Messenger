using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.Services;
using Microsoft.Extensions.Logging;
using CZ.TUL.PWA.Messenger.Server.Extensions;
using CZ.TUL.PWA.Messenger.Server.ViewModels;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly MessengerContext context;
        private readonly ITokenService tokenService;
        private readonly ILogger<MessagesController> logger;

        public MessagesController(MessengerContext context, ITokenService tokenService, ILogger<MessagesController> logger)
        {
            this.context = context;
            this.tokenService = tokenService;
            this.logger = logger;
        }

        // GET: api/Messages
        [HttpGet]
        public async Task<IEnumerable<MessageViewModel>> GetMessages([FromQuery] int limit = 50, [FromQuery]int offset = 0)
        {
            string userId = (await this.tokenService.GetCurrentUser(this.User)).Id;

            return await this.context.Messages.Where(x => x.OwnerId == userId)
                .Include(x => x.Conversation)
                .Include(x => x.Owner)
                .Skip(offset)
                .Take(limit)
                .Select(x => x.ToViewModel())
                .ToArrayAsync();
        }

        [HttpGet("Conversation/{id}")]
        public async Task<IEnumerable<MessageViewModel>> GetMessagesOfConversation(
            [FromRoute] int id,
            [FromQuery] int limit = 50,
            [FromQuery]int offset = 0)
        {
            string userId = (await this.tokenService.GetCurrentUser(this.User)).Id;

            return await this.context.Messages.Where(x => x.OwnerId == userId
                                                     && x.Conversation.ConversationId == id)
                .Include(x => x.Conversation)
                .Include(x => x.Owner)
                .Skip(offset)
                .Take(limit)
                .Select(x => x.ToViewModel())
                .ToArrayAsync();
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessage([FromRoute] int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            string userId = (await this.tokenService.GetCurrentUser(this.User)).Id;

            var message = await this.context.Messages
                                    .Include(x => x.Conversation)
                                    .Include(x => x.Owner)
                                    .SingleOrDefaultAsync(x => x.OwnerId == userId
                                                                            && x.MessageId == id);

            if (message == null)
            {
                return this.NotFound();
            }

            return this.Ok(message.ToViewModel());
        }

        // PUT: api/Messages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage([FromRoute] int id, [FromBody] Message message)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (id != message.MessageId)
            {
                return this.BadRequest();
            }

            string userId = (await this.tokenService.GetCurrentUser(this.User)).Id;
            if (this.context.Messages.Any(x => x.MessageId == id && x.OwnerId == userId) == false)
            {
                return this.BadRequest("Message not belongs to user");
            }

            this.context.Entry(message).State = EntityState.Modified;

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!this.MessageExists(id))
                {
                    return this.NotFound();
                }
                else
                {
                    this.logger.LogError("Unable to update Message", e);

                    throw;
                }
            }

            return this.NoContent();
        }

        // POST: api/Messages
        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] Message message)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            this.context.Messages.Add(message);
            await this.context.SaveChangesAsync();

            return this.CreatedAtAction("GetMessage", new { id = message.MessageId }, message.ToViewModel());
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage([FromRoute] int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            string userId = (await this.tokenService.GetCurrentUser(this.User)).Id;
            var message = await this.context.Messages.SingleOrDefaultAsync(x => x.OwnerId == userId
                                                                            && x.MessageId == id);
            if (message == null)
            {
                return this.NotFound();
            }

            this.context.Messages.Remove(message);
            await this.context.SaveChangesAsync();

            return this.Ok();
        }

        private bool MessageExists(int id)
        {
            return this.context.Messages.Any(e => e.MessageId == id);
        }
    }
}