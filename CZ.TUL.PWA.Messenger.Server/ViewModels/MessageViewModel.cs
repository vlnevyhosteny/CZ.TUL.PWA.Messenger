using CZ.TUL.PWA.Messenger.Server.Model;
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

        public User Owner
        {
            get;
            set;
        }

        public Conversation Conversation
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
