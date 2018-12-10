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
using Microsoft.AspNetCore.Identity;

namespace CZ.TUL.PWA.Messenger.Server.Hubs
{
    public class MessengerHub : Hub
    {
        private readonly MessengerContext context;
        private readonly ILogger<MessengerHub> logger;
        private readonly UserManager<User> userManager;

        public MessengerHub(
            MessengerContext context, 
            ILogger<MessengerHub> logger,
            UserManager<User> userManager)
        {
            this.context = context;
            this.logger = logger;
            this.userManager = userManager;
        }

        public override Task OnConnectedAsync()
        {
            var userName = this.Context.User.Identity.Name;

            User user = this.context.Users
                                    .Include(x => x.HubConnections)
                                    .SingleOrDefault(x => x.UserName == userName);
            if(user == null)
            {
                // TODO throw
            }

            user.HubConnections.Add(new HubConnection
            {
                HubConnectionId = this.Context.ConnectionId,
                Connected = true
            });
            this.context.SaveChangesAsync();

            return base.OnConnectedAsync();
        }

        [Authorize]
        public async Task ConversationUpdate(int conversationId)
        {
            Conversation conversation = await this.context.Conversations
                                                    .Include(x => x.UserConversations)
                                                    .SingleOrDefaultAsync(x => x.ConversationId == conversationId);

            if (conversation == null)
            {
                return;
            }

            List<string> addressesClientIds = conversation.UserConversations
                .Select(x => x.UserId)
                .ToList();

            foreach (var userId in addressesClientIds)
            {
                var addresse = this.context.Users
                                           .Include(x => x.HubConnections)
                                           .SingleOrDefault(x => x.Id == userId);

                if (addresse != null)
                {
                    foreach (var connection in addresse.HubConnections)
                    {
                        if (connection.Connected)
                        {
                            await this.Clients.Client(connection.HubConnectionId)
                                              .SendAsync("broadcastConversation", conversation.ToViewModel());
                        }
                    }
                }
            }
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
                .Select(x => x.UserId)
                .ToList();

            foreach (var userId in addressesClientIds)
            {
                var addresse = this.context.Users
                                           .Include(x => x.HubConnections)
                                           .SingleOrDefault(x => x.Id == userId);

                if (addresse != null)
                {
                    foreach (var connection in addresse.HubConnections)
                    {
                        if (connection.Connected)
                        {
                            await this.Clients.Client(connection.HubConnectionId)
                                              .SendAsync("broadcastMessage", messageDb.ToFlattenViewModel());
                        }
                    }
                } 
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            HubConnection connection = this.context.HubConnections.Find(this.Context.ConnectionId);
            this.context.Remove(connection);
            this.context.SaveChanges();

            return base.OnDisconnectedAsync(exception);
        }
    }
}
