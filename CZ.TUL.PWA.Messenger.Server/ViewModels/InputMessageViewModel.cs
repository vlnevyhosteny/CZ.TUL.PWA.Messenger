using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CZ.TUL.PWA.Messenger.Server.ViewModels
{
    public class InputMessageViewModel
    {
        public int ConversationId { get; set; }

        public string UserId { get; set; }

        public string Content { get; set; }
    }
}
