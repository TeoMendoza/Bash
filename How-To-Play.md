## Overview
This document covers how to play Bash and how to set it up locally or over LAN. There are two ways to play; a browser-based version hosted on Itch.io requiring zero setup, or a self-hosted local setup with full access to the codebase for lower latency and more control.

---

## Play on Itch.io
Bash is published to Itch.io and playable directly in the browser with no installation required.

> **Note:** Bash is deployed on SpacetimeDB's cloud infrastructure with US East as the only server location. Latency will scale with your geographic distance. If this is a concern, see Set Up Locally below.

**[Play Bash on Itch.io](https://teomendoza.itch.io/bash)**

---

## Set Up Locally
For lower latency, LAN play, or to explore the codebase, you can clone and run Bash locally.

### Prerequisites
- [Unity Hub](https://docs.unity.com/en-us/hub/install-hub)
- [SpacetimeDB CLI + Account](https://spacetimedb.com/docs/)

### Steps

1. Clone the repository and add the `client-unity` directory as a project in Unity Hub
2. Open the Unity project and open the repository root in your preferred IDE
3. Open two terminals, one at the repo root, one at `server-rust/spacetimedb/`
4. In the root terminal, run `spacetime start` to start the local server. The CLI may prompt you to sign in
5. In `client-unity/Assets/Scripts/GameManager.cs`, set `server_url` at the top of the file to `"http://127.0.0.1:3000"` for local play, or `"http://<your_private_ip>:<port>"` for LAN
6. In the `spacetimedb/` terminal, publish the module. For local, run `spacetime publish --server local bash`. For LAN, first run `spacetime server add --url http://<your_private_ip>:<port> self-host`, then `spacetime publish --server self-host bash`
7. Hit Play in Unity, you are connected to your local server. For LAN multiplayer, build an executable for your target OS and share it with players on your network

> **Note:** Self-hosted multiplayer only works over a local network.

---

## Controls

| Action | Input |
|---|---|
| Move | `WASD` |
| Jump | `Space` |
| Crouch | `Ctrl` |
| Sprint | `Shift` |
| View Character Kit | `I` |
| Start Match | `P` |
| Menu / Leave Game | `Tab` |

**View Character Kit (`I`):** Displays a description of your character's abilities and kit.

**Start Match (`P`):** Locks the session, no new players can join after a match starts. Enables the game timer and scoring.

**Menu (`Tab`):** Unlocks your cursor and opens the menu. Select Leave Game to exit.
