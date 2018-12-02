using System.Linq;
using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.ViewModels;

namespace CZ.TUL.PWA.Messenger.Server.Extensions
{
    public static class ConversationExtensions
    {
        public static ConversationViewModel ToViewModel(this Conversation conversation)
        {
            return new ConversationViewModel
            {
                ConversationId = conversation.ConversationId,
                Name = conversation.Name,
                Addressees = conversation.UserConversations?
                                         .Select(x => x.User.ToViewModel())
            };
        }
    }
}
