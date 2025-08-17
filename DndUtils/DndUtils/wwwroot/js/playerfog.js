window.initializePlayerFog = (width, height) => {
    const canvas = document.getElementById("playerCanvas");
    canvas.width = width;
    canvas.height = height;
    const ctx = canvas.getContext("2d");
    ctx.fillStyle = "rgba(176,176,176,1)";
    ctx.fillRect(0, 0, width, height);
};

window.startPlayerHubConnection = async () => {
    if (window.mapHubConnection) return;

    window.mapHubConnection = new signalR.HubConnectionBuilder()
        .withUrl("/mapHub")
        .withAutomaticReconnect()
        .build();

    await window.mapHubConnection.start();
    console.log("Player Hub connected");

    const canvas = document.getElementById("playerCanvas");
    const ctx = canvas.getContext("2d");

    // Draw chunks sent from DM
    window.mapHubConnection.on("ReceiveFogUpdate", (base64Chunk, x, y, width, height) => {
        const img = new Image();
        img.onload = () => {
            // Draw to temporary canvas
            const temp = document.createElement("canvas");
            temp.width = img.width;
            temp.height = img.height;
            const tempCtx = temp.getContext("2d");
            tempCtx.drawImage(img, 0, 0);

            // Copy pixels directly to player canvas
            const imgData = tempCtx.getImageData(0, 0, temp.width, temp.height);
            const data = imgData.data;

            // Draw processed image data to player canvas
            ctx.putImageData(imgData, x, y);
        };
        img.src = base64Chunk;
    });

    // Request full fog on first connection
    await window.mapHubConnection.invoke("RequestFullFog");
};
