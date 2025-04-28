using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace facetrackr_backend.Hubs
{
    public class VideoHub : Hub
    {
        public async Task SendOffer(string user, string offer)
        {
            await Clients.User(user).SendAsync("ReceiveOffer", offer);
        }

        public async Task SendAnswer(string user, string answer)
        {
            await Clients.User(user).SendAsync("ReceiveAnswer", answer);
        }

        public async Task SendIceCandidate(string user, string candidate)
        {
            await Clients.User(user).SendAsync("ReceiveIceCandidate", candidate);
        }

        public override Task OnConnectedAsync()
        {
            // Optionally handle connection events
            return base.OnConnectedAsync();
        }
    }
}
