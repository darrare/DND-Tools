window.startPlayerHubConnection = async (roomId) => {
    if (window.mapHubConnection) return;

    window.mapHubConnection = new signalR.HubConnectionBuilder()
        .withUrl("/mapHub")
        .withAutomaticReconnect()
        .build();

    await window.mapHubConnection.start();
    console.log("Player Hub connected");

    const canvas = document.getElementById("playerCanvas");
    const ctx = canvas.getContext("2d");

    // Listen for full fog snapshot from DM
    window.mapHubConnection.on("ReceiveFullFog", (base64Image) => {
        const img = new Image();
        img.onload = () => {
            canvas.width = img.width;
            canvas.height = img.height;
            ctx.drawImage(img, 0, 0);
        };
        img.src = base64Image;
    });

    // Listen for differential updates
    window.mapHubConnection.on("ReceiveFogUpdate", (base64Chunk, x, y, width, height) => {
        const img = new Image();
        img.onload = () => ctx.drawImage(img, x, y, width, height);
        img.src = base64Chunk;
    });

    // Join the room
    await window.mapHubConnection.invoke("JoinRoom", roomId, false);
};
