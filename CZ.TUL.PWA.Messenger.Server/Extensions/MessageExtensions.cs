using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.ViewModels;

namespace CZ.TUL.PWA.Messenger.Server.Extensions
{
    public static class MessageExtensions
    {
        public static MessageViewModel ToViewModel(this Message message)
        {
            return new MessageViewModel
            {
                MessageId = message.MessageId,
                Owner = message.Owner?.ToViewModel(),
                Conversation = message.Conversation?.ToViewModel(),
                Content = message.Content,
                DateSent = message.DateSent
            };
        }

        public static FlattenMessageViewModel ToFlattenViewModel(this Message message)
        {
            return new FlattenMessageViewModel
            {
                MessageId = message.MessageId,
                OwnerId = message.OwnerId,
                ConversationId = message.Conversation.ConversationId,
                Content = message.Content,
                DateSent = message.DateSent,
                ConversationName = message.Conversation.Name,
                Name = message.Owner.Name,
                UserName = message.Owner.UserName
            };
        }
    }
}
