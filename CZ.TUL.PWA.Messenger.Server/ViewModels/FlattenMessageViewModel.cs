using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CZ.TUL.PWA.Messenger.Server.ViewModels
{
    public class FlattenMessageViewModel
    {
        public int MessageId { get; set; }

        public DateTime DateSent { get; set; }

        public string Content { get; set; }

        public string OwnerId { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public int ConversationId { get; set; }

        public string ConversationName { get; set; }
    }
}
