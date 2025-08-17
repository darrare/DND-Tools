window.getImageNaturalSize = (img) => [img.naturalWidth, img.naturalHeight];

window.initializeFog = (canvas, width, height) => {
    canvas.width = width;
    canvas.height = height;

    const ctx = canvas.getContext("2d");
    canvas._ctx = ctx;

    // Fill canvas with semi-transparent black
    ctx.fillStyle = "rgba(0,0,0,0.7)";
    ctx.fillRect(0, 0, width, height);
};

// DM paints on canvas and sends updates to players
window.paintFog = (canvas, x, y, radius, erase) => {
    const ctx = canvas.getContext("2d");
    if (!ctx) return;

    const width = canvas.width;
    const height = canvas.height;

    const startX = Math.max(0, Math.floor(x - radius));
    const startY = Math.max(0, Math.floor(y - radius));
    const endX = Math.min(width, Math.ceil(x + radius));
    const endY = Math.min(height, Math.ceil(y + radius));

    const imgData = ctx.getImageData(startX, startY, endX - startX, endY - startY);
    const data = imgData.data;

    for (let j = 0; j < imgData.height; j++) {
        for (let i = 0; i < imgData.width; i++) {
            const dx = (startX + i) - x;
            const dy = (startY + j) - y;

            if (dx * dx + dy * dy <= radius * radius) {
                const index = (j * imgData.width + i) * 4;

                if (erase) data[index + 3] = 0; // reveal
                else data[index + 3] = 178;    // 0.7 alpha
            }
        }
    }

    ctx.putImageData(imgData, startX, startY);

    // Send update via SignalR
    if (window.mapHubConnection) {
        const tempCanvas = document.createElement("canvas");
        tempCanvas.width = endX - startX;
        tempCanvas.height = endY - startY;
        tempCanvas.getContext("2d").putImageData(imgData, 0, 0);
        const base64Chunk = tempCanvas.toDataURL("image/png");

        window.mapHubConnection.invoke("UpdateFog", base64Chunk, startX, startY, endX - startX, endY - startY);
    }
};

// Connect to SignalR hub
window.startMapHubConnection = async () => {
    window.mapHubConnection = new signalR.HubConnectionBuilder()
        .withUrl("/mapHub")
        .withAutomaticReconnect()
        .build();

    await window.mapHubConnection.start();

    // Listen for fog updates (players only)
    window.mapHubConnection.on("ReceiveFogUpdate", (base64Chunk, x, y, width, height) => {
        const img = new Image();
        img.onload = () => {
            const ctx = document.getElementById("playerCanvas").getContext("2d");
            ctx.drawImage(img, x, y);
        };
        img.src = base64Chunk;
    });
};

// Optional: Save DM fog layer as PNG
window.saveFogLayer = (canvas) => {
    const dataURL = canvas.toDataURL("image/png");
    console.log("Fog saved as PNG", dataURL); // You can send to server
};
