using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CZ.TUL.PWA.Messenger.Server.Model
{
    public class User : IdentityUser
    {
        [Required]
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

        public List<UserConversation> UserConversations
        {
            get;
            set;
        }
    }
}
