﻿using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CZ.TUL.PWA.Messenger.Server.Hubs
{
    [Authorize]
    public class MessengerHub : Hub
    {
        private readonly MessengerContext context;
        private readonly ILogger<MessengerHub> logger;

        public MessengerHub(MessengerContext context, ILogger<MessengerHub> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task SendAsync(InputMessageViewModel inputMessage)
        {
            Conversation conversation = await this.context.Conversations
                                                    .Include(x => x.UserConversations.Select(y => y.User))
                                                    .SingleOrDefaultAsync(x => x.ConversationId == inputMessage.ConversationId);

            if (conversation == null)
            {
                this.logger.LogInformation("Message with no conversation. Cannot be broadcasted.");

                return;
            }

            User owner = conversation.UserConversations.SingleOrDefault(x => x.UserId == inputMessage.UserId).User;
            if (owner == null)
            {
                this.logger.LogWarning("Meesage with no owner. Cannot be broadcasted");

                return;
            }

            await this.context.AddAsync(new Message()
            {
                Content = inputMessage.Content,
                Conversation = conversation,
                DateSent = DateTime.Now,
                Owner = owner
            });

            List<string> addressesClientIds = conversation.UserConversations
                .Where(x => x.UserId != inputMessage.UserId)
                .Select(x => x.UserId)
                .ToList();

            await this.Clients.Users(addressesClientIds).SendAsync("broadcastMessage", inputMessage);
        }
    }
}