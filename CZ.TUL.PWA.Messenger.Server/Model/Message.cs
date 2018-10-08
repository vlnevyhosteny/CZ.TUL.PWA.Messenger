using System;
namespace CZ.TUL.PWA.Messenger.Server.Model
{
    public class Message
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

        public string Content
        {
            get;
            set;
        }

        public DateTime DateSent
        {
            get;
            set;
        }
    }
}
