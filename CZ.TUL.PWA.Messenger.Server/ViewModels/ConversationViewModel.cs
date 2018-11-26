using System.Collections.Generic;
using CZ.TUL.PWA.Messenger.Server.Model;
namespace CZ.TUL.PWA.Messenger.Server.ViewModels
{
    public class ConversationViewModel
    {
        public int ConversationId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public IEnumerable<UserViewModel> Addressees
        {
            get;
            set;
        }
    }
}
