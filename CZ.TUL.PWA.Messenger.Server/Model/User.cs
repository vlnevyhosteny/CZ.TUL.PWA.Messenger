using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Column("Password")]
        public string PasswordHash
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
