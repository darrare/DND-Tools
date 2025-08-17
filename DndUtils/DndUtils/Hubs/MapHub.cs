using Microsoft.AspNetCore.SignalR;

namespace DndUtils
{
    public class MapHub : Hub
    {
        public async Task UpdateFog(string base64Chunk, int x, int y, int width, int height)
        {
            // Broadcast to all clients except sender (DM)
            await Clients.Others.SendAsync("ReceiveFogUpdate", base64Chunk, x, y, width, height);
        }
    }
}
