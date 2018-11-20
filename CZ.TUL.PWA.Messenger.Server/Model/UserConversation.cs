namespace CZ.TUL.PWA.Messenger.Server.Model
{
    public class UserConversation
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

        public User User
        {
            get;
            set;
        }

        public Conversation Conversation
        {
            get;
            set;
        }
    }
}
