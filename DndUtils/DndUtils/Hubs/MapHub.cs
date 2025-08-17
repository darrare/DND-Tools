using Microsoft.AspNetCore.SignalR;

namespace DndUtils;

public class MapHub : Hub
{
    // DM sends a fog chunk (or full canvas)
    public async Task UpdateFog(string base64Chunk, int x, int y, int width, int height)
    {
        await Clients.Others.SendAsync("ReceiveFogUpdate", base64Chunk, x, y, width, height);
    }

    // Player requests full fog when joining
    public async Task RequestFullFog()
    {
        // Ask DM client to send full fog to this player
        await Clients.Others.SendAsync("SendFullFogToPlayer", Context.ConnectionId);
    }
}
