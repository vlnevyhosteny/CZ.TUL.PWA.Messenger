using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.Services;
using Microsoft.Extensions.Logging;
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using CZ.TUL.PWA.Messenger.Server.Extensions;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserConversationsController : ControllerBase
    {
        private readonly MessengerContext context;
        private readonly ITokenService tokenService;
        private readonly ILogger<UserConversationsController> logger;

        public UserConversationsController(MessengerContext context, TokenService tokenService, ILogger<UserConversationsController> logger)
        {
            this.context = context;
            this.tokenService = tokenService;
            this.logger = logger;
        }

        // GET: api/UserConversations
        [HttpGet]
        public async Task<IEnumerable<UserConversation>> GetUserConversation([FromQuery] int limit = 100, [FromQuery]int offset = 0)
        {
            string userId = await this.tokenService.GetCurrentUserId(this.User);

            return await this.context.UserConversations.Where(x => x.UserId == userId)
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync();
        }

        [HttpGet("owned")]
        public async Task<IEnumerable<UserConversation>> GetOwnedUserConversation([FromQuery] int limit = 100, [FromQuery]int offset = 0)
        {
            string userId = await this.tokenService.GetCurrentUserId(this.User);

            return await this.context.UserConversations.Where(x => x.UserId == userId && x.IsOwner)
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync();
        }

        // GET: api/UserConversations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserConversation([FromRoute] int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            string userId = await this.tokenService.GetCurrentUserId(this.User);

            var userConversation = await this.context.UserConversations.SingleOrDefaultAsync(x => x.UserId == userId
                                                                                            && x.ConversationId == id);

            if (userConversation == null)
            {
                return this.NotFound();
            }

            return this.Ok(userConversation);
        }

        // PUT: api/UserConversations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserConversation([FromRoute] int id, [FromBody] UserConversation userConversation)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (id != userConversation.ConversationId)
            {
                return this.BadRequest();
            }

            string userId = await this.tokenService.GetCurrentUserId(this.User);
            if (this.context.UserConversations.Any(x => x.ConversationId == id && x.UserId == userId && x.IsOwner) == false)
            {
                return this.BadRequest("UserConversation not belongs to user");
            }

            this.context.Entry(userConversation).State = EntityState.Modified;

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!this.UserConversationExists(id))
                {
                    return this.NotFound();
                }
                else
                {
                    this.logger.LogError("Unable to update UserConversation", e);

                    throw;
                }
            }

            return this.NoContent();
        }

        // POST: api/UserConversations
        [HttpPost]
        public async Task<IActionResult> PostUserConversation([FromBody] UserConversation userConversation)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            this.context.UserConversations.Add(userConversation);
            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (this.UserConversationExists(userConversation.ConversationId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return this.CreatedAtAction("GetUserConversation", new { id = userConversation.ConversationId }, userConversation);
        }

        // DELETE: api/UserConversations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserConversation([FromRoute] int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            string userId = await this.tokenService.GetCurrentUserId(this.User);
            var userConversation = await this.context.UserConversations.SingleOrDefaultAsync(x => x.UserId == userId
                                                                                            && x.ConversationId == id);
            if (userConversation == null)
            {
                return this.NotFound();
            }

            this.context.UserConversations.Remove(userConversation);
            await this.context.SaveChangesAsync();

            return this.Ok(userConversation);
        }

        private bool UserConversationExists(int id)
        {
            return this.context.UserConversations.Any(e => e.ConversationId == id);
        }
    }
}