using System;
using System.ComponentModel.DataAnnotations;

namespace CZ.TUL.PWA.Messenger.Server.ViewModels
{
    public class MessageViewModel
    {
        public int MessageId
        {
            get;
            set;
        }

        public UserViewModel Owner
        {
            get;
            set;
        }

        public ConversationViewModel Conversation
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        [Timestamp]
        public DateTime DateSent
        {
            get;
            set;
        }
    }
}
