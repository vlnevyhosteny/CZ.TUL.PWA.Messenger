using System;
namespace CZ.TUL.PWA.Messenger.Server.Model
{
    public class HubConnection
    {
        public string HubConnectionId { get; set; }

        public string UserAnget { get; set; }

        public bool Connected { get; set; }
    }
}
