using System;
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
    }
}
