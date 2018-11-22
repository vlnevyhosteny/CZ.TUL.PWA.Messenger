using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CZ.TUL.PWA.Messenger.Server.Model
{
    public class Conversation
    {
        public int ConversationId
        {
            get;
            set;
        }

        [MaxLength(100)]
        public string Name
        {
            get;
            set;
        }

        public List<Message> Messages
        {
            get;
            set;
        }

        public List<UserConversation> UserConversations { get; set; }
    }
}
