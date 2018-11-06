using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CZ.TUL.PWA.Messenger.Server.Model;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly MessengerContext context;

        public MessagesController(MessengerContext context)
        {
            this.context = context;
        }

        // GET: api/Messages
        [HttpGet]
        public IEnumerable<Message> GetMessages()
        {
            return this.context.Messages;
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessage([FromRoute] int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var message = await this.context.Messages.FindAsync(id);

            if (message == null)
            {
                return this.NotFound();
            }

            return this.Ok(message);
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

            this.context.Entry(message).State = EntityState.Modified;

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.MessageExists(id))
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

            return this.CreatedAtAction("GetMessage", new { id = message.MessageId }, message);
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage([FromRoute] int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var message = await this.context.Messages.FindAsync(id);
            if (message == null)
            {
                return this.NotFound();
            }

            this.context.Messages.Remove(message);
            await this.context.SaveChangesAsync();

            return this.Ok(message);
        }

        private bool MessageExists(int id)
        {
            return this.context.Messages.Any(e => e.MessageId == id);
        }
    }
}