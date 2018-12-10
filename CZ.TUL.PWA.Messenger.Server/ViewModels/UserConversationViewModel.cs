using System;
namespace CZ.TUL.PWA.Messenger.Server.ViewModels
{
    public class UserConversationViewModel
    {
        public string UserId
        {
            get;
            set;
        }

        public int ConversationId
        {
            get;
            set;
        }

        public bool IsOwner
        {
            get;
            set;
        }

        public bool NotRead
        {
            get;
            set;
        }

        public int NotReadCount
        {
            get;
            set;
        }
    }
}
