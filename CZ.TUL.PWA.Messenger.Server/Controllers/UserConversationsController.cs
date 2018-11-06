using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CZ.TUL.PWA.Messenger.Server.Model;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserConversationsController : ControllerBase
    {
        private readonly MessengerContext context;

        public UserConversationsController(MessengerContext context)
        {
            this.context = context;
        }

        // GET: api/UserConversations
        [HttpGet]
        public IEnumerable<UserConversation> GetUserConversation()
        {
            return this.context.UserConversation;
        }

        // GET: api/UserConversations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserConversation([FromRoute] int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var userConversation = await this.context.UserConversation.FindAsync(id);

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

            this.context.Entry(userConversation).State = EntityState.Modified;

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.UserConversationExists(id))
                {
                    return this.NotFound();
                }
                else
                {
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

            this.context.UserConversation.Add(userConversation);
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

            var userConversation = await this.context.UserConversation.FindAsync(id);
            if (userConversation == null)
            {
                return this.NotFound();
            }

            this.context.UserConversation.Remove(userConversation);
            await this.context.SaveChangesAsync();

            return this.Ok(userConversation);
        }

        private bool UserConversationExists(int id)
        {
            return this.context.UserConversation.Any(e => e.ConversationId == id);
        }
    }
}