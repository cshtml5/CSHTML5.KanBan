using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;

namespace DemoSignalRServer
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        public void MakeCallToUpdateOtherClients()
        {
            Clients.Others.UpdateKanBan();
        }

        public override async Task OnConnected()
        {
            await Groups.Add(Context.ConnectionId, "SignalR Users");
            await base.OnConnected();
        }
    }
}