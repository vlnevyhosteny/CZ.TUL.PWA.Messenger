using System;
using Microsoft.AspNetCore.SignalR;

namespace CZ.TUL.PWA.Messenger.Server.Hubs
{
    public class ChatHub : Hub
    {
        public void SendToAll(string message)
        {
            Clients.All.SendAsync("Send", message);
        }
    }
}
