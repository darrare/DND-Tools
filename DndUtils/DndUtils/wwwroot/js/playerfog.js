window.initializePlayerFog = (width, height) => {
    const canvas = document.getElementById("playerCanvas");
    canvas.width = width;
    canvas.height = height;
    const ctx = canvas.getContext("2d");
    canvas._ctx = ctx;

    // Fill the player canvas with solid grey fog
    ctx.fillStyle = "grey";
    ctx.fillRect(0, 0, width, height);
};

window.startMapHubConnection = async (isPlayer = false) => {
    if (window.mapHubConnection) return;

    window.mapHubConnection = new signalR.HubConnectionBuilder()
        .withUrl("/mapHub")
        .withAutomaticReconnect()
        .build();

    window.mapHubConnection.onclose(() => console.log("Hub closed"));

    await window.mapHubConnection.start();
    console.log("Hub connection started"); // <-- check console

    if (isPlayer) {
        window.mapHubConnection.on("ReceiveFogUpdate", (base64Chunk, x, y, width, height) => {
            const img = new Image();
            img.onload = () => {
                const ctx = document.getElementById("playerCanvas").getContext("2d");
                ctx.drawImage(img, x, y);
            };
            img.src = base64Chunk;
        });
    }
};