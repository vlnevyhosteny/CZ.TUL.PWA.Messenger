using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using CZ.TUL.PWA.Messenger.Server.Extensions;

namespace CZ.TUL.PWA.Messenger.Server.Hubs
{
    public class MessengerHub : Hub
    {
        private readonly MessengerContext context;
        private readonly ILogger<MessengerHub> logger;

        public MessengerHub(MessengerContext context, ILogger<MessengerHub> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        [Authorize]
        public async Task Send(InputMessageViewModel inputMessage)
        {
            Conversation conversation = await this.context.Conversations
                                                    .Include(x => x.UserConversations)
                                                    .SingleOrDefaultAsync(x => x.ConversationId == inputMessage.ConversationId);

            if (conversation == null)
            {
                this.logger.LogInformation("Message with no conversation. Cannot be broadcasted.");

                return;
            }

            User owner = await this.context.Users.FindAsync(inputMessage.UserId);
            if (owner == null)
            {
                this.logger.LogWarning("Meesage with no owner. Cannot be broadcasted");

                return;
            }

            var messageDb = new Message
            {
                Content = inputMessage.Content,
                Conversation = conversation,
                Owner = owner
            };

            try
            {
                await this.context.AddAsync(messageDb);
                await this.context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError("Problem with storing message", ex);
            }

            List<string> addressesClientIds = conversation.UserConversations
                .Where(x => x.UserId != inputMessage.UserId)
                .Select(x => x.UserId)
                .ToList();
                
            await this.Clients.All.SendAsync("broadcastMessage", messageDb.ToFlattenViewModel());
        }
    }
}
