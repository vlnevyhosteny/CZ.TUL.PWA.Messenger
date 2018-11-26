using System;
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using CZ.TUL.PWA.Messenger.Server.Model;
namespace CZ.TUL.PWA.Messenger.Server.Extensions
{
    public static class ConversationExtensions
    {
        public static ConversationViewModel ToViewModel(this Conversation conversation) 
        {
            return new ConversationViewModel
            {
                ConversationId = conversation.ConversationId,
                Name = conversation.Name
            };
        }
    }
}
