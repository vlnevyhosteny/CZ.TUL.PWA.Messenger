using System;
using System.Threading.Tasks;
using CZ.TUL.PWA.Messenger.Server.Model;
using Microsoft.EntityFrameworkCore.Internal;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
namespace CZ.TUL.PWA.Messenger.Server.Services
{
    public class ConversationService : IConversationService
    {
        private readonly MessengerContext messengerContext;

        public ConversationService(MessengerContext messengerContext)
        {
            this.messengerContext = messengerContext;
        }

        public async Task<bool> BelongsToUser(string userId, int conversationId)
        {
            return await this.messengerContext.UserConversations.AnyAsync(x => x.ConversationId == conversationId
                                                                          && x.UserId == userId
                                                                          && x.IsOwner);
        }
    }
}
