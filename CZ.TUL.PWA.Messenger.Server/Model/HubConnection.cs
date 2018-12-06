using System;
namespace CZ.TUL.PWA.Messenger.Server.Model
{
    public class HubConnection
    {
        public string HubConnectionId { get; set; }

        public string UserAgent { get; set; }

        public bool Connected { get; set; }
    }
}
