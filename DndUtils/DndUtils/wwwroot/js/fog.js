window.getImageNaturalSize = (img) => [img.naturalWidth, img.naturalHeight];

window.initializeFog = (canvas, width, height) => {
    canvas.width = width;
    canvas.height = height;
    const ctx = canvas.getContext("2d");
    canvas._ctx = ctx;

    const offscreen = document.createElement("canvas");
    offscreen.width = width;
    offscreen.height = height;
    const offCtx = offscreen.getContext("2d");

    offCtx.fillStyle = "rgba(0,0,0,0.7)";
    offCtx.fillRect(0, 0, width, height);

    canvas._offscreen = offscreen;
    canvas._offCtx = offCtx;

    ctx.clearRect(0, 0, width, height);
    ctx.drawImage(offscreen, 0, 0);
};

window.paintFog = async (canvas, x, y, radius, erase) => {
    const ctx = canvas.getContext("2d");
    const width = canvas.width;
    const height = canvas.height;

    const startX = Math.max(0, Math.floor(x - radius));
    const startY = Math.max(0, Math.floor(y - radius));
    const endX = Math.min(width, Math.ceil(x + radius));
    const endY = Math.min(height, Math.ceil(y + radius));

    const imgData = ctx.getImageData(startX, startY, endX - startX, endY - startY);
    const playerImageData = ctx.getImageData(startX, startY, endX - startX, endY - startY);

    // Only change color of the outer ring to make it circular
    for (let j = 0; j < imgData.height; j++) {
        for (let i = 0; i < imgData.width; i++) {
            const dx = (startX + i) - x;
            const dy = (startY + j) - y;
            const index = (j * imgData.width + i) * 4;
            if (dx * dx + dy * dy <= radius * radius) {
                if (erase) {
                    imgData.data[index + 3] = 0;
                }
                else {
                    imgData.data[index] = 0;
                    imgData.data[index + 1] = 0;
                    imgData.data[index + 2] = 0;
                    imgData.data[index + 3] = 178;
                }
            }
            // Update playerdata so we send something slightly different to the player.
            if (imgData.data[index + 3] == 0) {
                playerImageData.data[index + 3] = 0;
            } else {
                playerImageData.data[index] = 176;
                playerImageData.data[index + 1] = 176;
                playerImageData.data[index + 2] = 176;
                playerImageData.data[index + 3] = 255;
            }
        }
    }

    ctx.putImageData(imgData, startX, startY);

    if (window.mapHubConnection) {
        // Instead of sending only fog alpha, draw the map + fog onto a temp canvas
        const tempCanvas = document.createElement("canvas");
        tempCanvas.width = endX - startX;
        tempCanvas.height = endY - startY;
        const tempCtx = tempCanvas.getContext("2d");

        // Draw map portion
        const mapImg = document.getElementById("mapImage");
        tempCtx.drawImage(mapImg, startX, startY, endX - startX, endY - startY, 0, 0, endX - startX, endY - startY);

        // Draw fog portion from playerData with alpha
        const fogCanvas = document.createElement("canvas");
        fogCanvas.width = playerImageData.width;
        fogCanvas.height = playerImageData.height;
        fogCanvas.getContext("2d").putImageData(playerImageData, 0, 0);

        // Draw fog on top of map
        tempCtx.drawImage(fogCanvas, 0, 0);

        const base64Chunk = tempCanvas.toDataURL("image/png");
        await window.mapHubConnection.invoke("UpdateFog", base64Chunk, startX, startY, endX - startX, endY - startY);
    }
};

// Store current fog for new players
window.storeFullFog = () => {
    const canvas = document.getElementById("fogCanvas");
    window._dmFullFog = canvas.toDataURL("image/png");
};

// Send full fog to all current players
window.sendFullFogToPlayer = async () => {
    if (!window.mapHubConnection) return;
    const canvas = document.getElementById("fogCanvas");
    const base64 = window._dmFullFog ?? canvas.toDataURL("image/png");
    await window.mapHubConnection.invoke("UpdateFog", base64, 0, 0, canvas.width, canvas.height);
};

// Send full fog to a new player on request
window.sendFullFogToNewPlayer = async (connectionId) => {
    if (!window.mapHubConnection || !window._dmFullFog) return;
    const canvas = document.getElementById("fogCanvas");
    await window.mapHubConnection.invoke("UpdateFog", window._dmFullFog, 0, 0, canvas.width, canvas.height);
};

// Start DM hub connection
window.startMapHubConnection = async () => {
    if (window.mapHubConnection) return;

    window.mapHubConnection = new signalR.HubConnectionBuilder()
        .withUrl("/mapHub")
        .withAutomaticReconnect()
        .build();

    window.mapHubConnection.on("SendFullFogToPlayer", async (connectionId) => {
        await window.sendFullFogToNewPlayer(connectionId);
    });

    await window.mapHubConnection.start();
    console.log("DM Hub connected");
};
