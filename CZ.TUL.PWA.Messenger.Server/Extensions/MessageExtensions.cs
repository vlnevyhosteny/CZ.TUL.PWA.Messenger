﻿using CZ.TUL.PWA.Messenger.Server.ViewModels;
using CZ.TUL.PWA.Messenger.Server.Model;

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
    }
}
