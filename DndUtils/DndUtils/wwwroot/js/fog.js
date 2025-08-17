window.getImageNaturalSize = (img) => {
    return [img.naturalWidth, img.naturalHeight];
}

window.initializeFog = (canvas, width, height) => {
    canvas.width = width;
    canvas.height = height;

    const ctx = canvas.getContext("2d");
    canvas._ctx = ctx;

    // Create an offscreen canvas to store fog layer
    const offscreen = document.createElement("canvas");
    offscreen.width = width;
    offscreen.height = height;
    const offCtx = offscreen.getContext("2d");

    // Fill offscreen with semi-transparent black
    offCtx.fillStyle = "rgba(0,0,0,0.7)";
    offCtx.fillRect(0, 0, width, height);

    canvas._offscreen = offscreen;
    canvas._offCtx = offCtx;

    // Draw the offscreen onto visible canvas
    ctx.clearRect(0, 0, width, height);
    ctx.drawImage(offscreen, 0, 0);
};

window.paintFog = (canvas, x, y, radius, erase) => {
    const ctx = canvas.getContext("2d");
    if (!ctx) return;

    const width = canvas.width;
    const height = canvas.height;

    // Compute bounds of the square we will read
    const startX = Math.max(0, Math.floor(x - radius));
    const startY = Math.max(0, Math.floor(y - radius));
    const endX = Math.min(width, Math.ceil(x + radius));
    const endY = Math.min(height, Math.ceil(y + radius));

    const imgData = ctx.getImageData(startX, startY, endX - startX, endY - startY);
    const data = imgData.data;

    for (let j = 0; j < imgData.height; j++) {
        for (let i = 0; i < imgData.width; i++) {
            // Compute distance from brush center
            const dx = (startX + i) - x;
            const dy = (startY + j) - y;

            if (dx * dx + dy * dy <= radius * radius) {
                const index = (j * imgData.width + i) * 4;

                if (erase) {
                    data[index + 3] = 0; // alpha = 0 -> reveal
                } else {
                    data[index] = 0;     // R
                    data[index + 1] = 0; // G
                    data[index + 2] = 0; // B
                    data[index + 3] = 178; // alpha = 0.7*255
                }
            }
        }
    }

    ctx.putImageData(imgData, startX, startY);
};

