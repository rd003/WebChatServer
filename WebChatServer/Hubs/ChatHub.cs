using Microsoft.AspNetCore.SignalR;

namespace WebChatServer.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<string,string> connectedClients= new Dictionary<string,string>();
        // this method will send notification to all clients
        // if client have to communicate, it will call <SendMessage()> method
        // if client have to receive notification from server it will use <ReceiveMessage> method.
        public async Task SendMessage(string user,string message)
        {
            await Clients.All.SendAsync("ReceiveMessage",user, message);
        }

        // Everyone will be notified except who have joined the chad
        public async Task JoinChat(string user, string message)
        {
            connectedClients[Context.ConnectionId] = user;
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }

        private async Task LeaveChat()
        {
            if (connectedClients.TryGetValue(Context.ConnectionId, out string user))
            {
                var message = $"{user} left the chat";
                await Clients.Others.SendAsync("ReceiveMessage", user, message);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await LeaveChat();
            await base.OnDisconnectedAsync(exception);

        }
    }
}
