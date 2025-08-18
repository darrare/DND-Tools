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
        //await window.mapHubConnection.invoke("UpdateFog", base64Chunk, startX, startY, endX - startX, endY - startY);
        broadcastFogChunk(base64Chunk, startX, startY, endX - startX, endY - startY);
    }
};

window.startDmHubConnection = async (roomId, canvasId) => {
    if (window.mapHubConnection) return;

    const canvas = document.getElementById(canvasId);

    window.mapHubConnection = new signalR.HubConnectionBuilder()
        .withUrl("/mapHub")
        .withAutomaticReconnect()
        .build();

    await window.mapHubConnection.start();
    console.log("DM Hub connected");

    // Join as DM
    await window.mapHubConnection.invoke("JoinRoom", roomId, true);

    // When a player joins, send them the full snapshot
    window.mapHubConnection.on("PlayerJoined", async (connectionId) => {
        // Create a composite canvas (map + fog)
        const composite = document.createElement("canvas");
        composite.width = fogCanvas.width;
        composite.height = fogCanvas.height;
        const compositeCtx = composite.getContext("2d");

        // Draw base map
        const mapImg = document.getElementById("mapImage");
        compositeCtx.drawImage(mapImg, 0, 0, composite.width, composite.height);

        // Overlay fog layer
        compositeCtx.drawImage(fogCanvas, 0, 0);

        updateCanvasColors(composite, compositeCtx);

        // Encode result
        const fullImageBase64 = composite.toDataURL("image/png");
        await window.mapHubConnection.invoke("SendFullFogToPlayer", connectionId, fullImageBase64);
    });

    // Broadcast chunks normally when you reveal fog
    window.broadcastFogChunk = async (base64Chunk, x, y, width, height) => {
        await window.mapHubConnection.invoke("BroadcastFogUpdate", roomId, base64Chunk, x, y, width, height);
    };
};

function updateCanvasColors(canvas, ctx) {
    // Get the image data (all pixels)
    const imgData = ctx.getImageData(0, 0, canvas.width, canvas.height);
    const data = imgData.data; // Uint8ClampedArray [r, g, b, a, r, g, b, a, ...]

    // Define your colors
    const colorB = { r: 0, g: 0, b: 0, a: 178 };   // the "old" color (B)
    const colorC = { r: 176, g: 176, b: 176, a: 1 }; // the "new" color

    // Loop over every pixel
    for (let i = 0; i < data.length; i += 4) {
        const r = data[i];
        const g = data[i + 1];
        const b = data[i + 2];
        const a = data[i + 3];

        // Check if pixel matches colorB
        if (r === colorB.r && g === colorB.g && b === colorB.b && a === colorB.a) {
            // Replace with colorC
            data[i] = colorC.r;
            data[i + 1] = colorC.g;
            data[i + 2] = colorC.b;
            data[i + 3] = colorC.a;
        }
    }

    // Push the modified data back onto the canvas
    ctx.putImageData(imgData, 0, 0);
}