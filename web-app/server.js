const fs = require("node:fs");
const path = require("node:path");
const http = require("node:http");
const readline = require("node:readline");
const { exec } = require("node:child_process");

const host = "127.0.0.1";
const port = Number(process.argv[2] || "5500");
const root = path.resolve(__dirname);
const baseUrl = `http://${host}:${port}`;

const mimeTypes = {
    ".html": "text/html; charset=utf-8",
    ".css": "text/css; charset=utf-8",
    ".js": "application/javascript; charset=utf-8",
    ".json": "application/json; charset=utf-8",
    ".svg": "image/svg+xml",
    ".png": "image/png",
    ".jpg": "image/jpeg",
    ".jpeg": "image/jpeg",
    ".ico": "image/x-icon"
};

function resolveFilePath(urlPath) {
    const cleanPath = decodeURIComponent(urlPath.split("?")[0]);
    const requested = cleanPath === "/" ? "/index.html" : cleanPath;
    const fullPath = path.join(root, requested);
    const normalized = path.normalize(fullPath);

    if (!normalized.startsWith(root)) {
        return null;
    }

    return normalized;
}

const server = http.createServer((request, response) => {
    const filePath = resolveFilePath(request.url || "/");
    if (!filePath) {
        response.writeHead(403, { "Content-Type": "text/plain; charset=utf-8" });
        response.end("Forbidden");
        return;
    }

    fs.stat(filePath, (statError, stats) => {
        if (statError || !stats.isFile()) {
            response.writeHead(404, { "Content-Type": "text/plain; charset=utf-8" });
            response.end("Not Found");
            return;
        }

        const extension = path.extname(filePath).toLowerCase();
        const mimeType = mimeTypes[extension] || "application/octet-stream";
        response.writeHead(200, { "Content-Type": mimeType });
        fs.createReadStream(filePath).pipe(response);
    });
});

let shuttingDown = false;

function shutdownServer() {
    if (shuttingDown) {
        return;
    }

    shuttingDown = true;
    server.close(() => {
        // eslint-disable-next-line no-console
        console.log("Server stopped.");
        process.exit(0);
    });
}

function openBrowser(url) {
    let command = "";
    if (process.platform === "win32") {
        command = `start "" "${url}"`;
    } else if (process.platform === "darwin") {
        command = `open "${url}"`;
    } else {
        command = `xdg-open "${url}"`;
    }

    exec(command, (error) => {
        if (error) {
            // eslint-disable-next-line no-console
            console.log(`Could not open browser automatically. Open manually: ${url}`);
        }
    });
}

function startInteractiveConsole() {
    if (!process.stdin.isTTY || !process.stdout.isTTY) {
        return;
    }

    // eslint-disable-next-line no-console
    console.log("Interactive commands: help | status | open | quit");

    const rl = readline.createInterface({
        input: process.stdin,
        output: process.stdout,
        prompt: "web-app> "
    });

    rl.prompt();

    rl.on("line", (line) => {
        const command = line.trim().toLowerCase();

        if (command === "help") {
            // eslint-disable-next-line no-console
            console.log("Commands: help, status, open, quit");
        } else if (command === "status") {
            // eslint-disable-next-line no-console
            console.log(`Serving ${root} at ${baseUrl}`);
        } else if (command === "open") {
            openBrowser(baseUrl);
        } else if (command === "quit" || command === "exit") {
            shutdownServer();
        } else if (command.length > 0) {
            // eslint-disable-next-line no-console
            console.log(`Unknown command: ${command}`);
        }

        if (!shuttingDown) {
            rl.prompt();
        }
    });

    rl.on("close", () => {
        shutdownServer();
    });
}

process.on("SIGINT", () => {
    shutdownServer();
});

server.listen(port, host, () => {
    // eslint-disable-next-line no-console
    console.log(`web-app running at ${baseUrl}`);
    startInteractiveConsole();
});
