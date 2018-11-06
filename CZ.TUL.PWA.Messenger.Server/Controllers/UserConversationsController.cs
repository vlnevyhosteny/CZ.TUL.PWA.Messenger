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
        private readonly MessengerContext _context;

        public UserConversationsController(MessengerContext context)
        {
            _context = context;
        }

        // GET: api/UserConversations
        [HttpGet]
        public IEnumerable<UserConversation> GetUserConversation()
        {
            return _context.UserConversation;
        }

        // GET: api/UserConversations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserConversation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userConversation = await _context.UserConversation.FindAsync(id);

            if (userConversation == null)
            {
                return NotFound();
            }

            return Ok(userConversation);
        }

        // PUT: api/UserConversations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserConversation([FromRoute] int id, [FromBody] UserConversation userConversation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userConversation.ConversationId)
            {
                return BadRequest();
            }

            _context.Entry(userConversation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserConversationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserConversations
        [HttpPost]
        public async Task<IActionResult> PostUserConversation([FromBody] UserConversation userConversation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UserConversation.Add(userConversation);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserConversationExists(userConversation.ConversationId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUserConversation", new { id = userConversation.ConversationId }, userConversation);
        }

        // DELETE: api/UserConversations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserConversation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userConversation = await _context.UserConversation.FindAsync(id);
            if (userConversation == null)
            {
                return NotFound();
            }

            _context.UserConversation.Remove(userConversation);
            await _context.SaveChangesAsync();

            return Ok(userConversation);
        }

        private bool UserConversationExists(int id)
        {
            return _context.UserConversation.Any(e => e.ConversationId == id);
        }
    }
}