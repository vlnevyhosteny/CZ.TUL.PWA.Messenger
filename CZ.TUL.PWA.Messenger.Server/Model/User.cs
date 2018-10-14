using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CZ.TUL.PWA.Messenger.Server.Model
{
    public class User
    {

        public int UserId
        {
            get;
            set;
        }

        [Required]
        public string UserName
        {
            get;
            set;
        }

        public string Password
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
