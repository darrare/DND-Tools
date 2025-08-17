using Microsoft.AspNetCore.SignalR;

namespace DndUtils
{
    public class MapHub : Hub
    {
        // DM sends updates to players
        public async Task UpdateFog(string base64Chunk, int x, int y, int width, int height)
        {
            await Clients.Others.SendAsync("ReceiveFogUpdate", base64Chunk, x, y, width, height);
        }
    }
}
