# Snake Game Project - Detailed Status and Guide

## Project Overview
This is a Blazor-based Snake game that was originally designed to work with:
1. A TCP server (for multiplayer game logic)
2. A MySQL database (for storing game statistics)
3. The game GUI connects to `localhost:11000` by default

## Current Status (as of last session)

### What's Been Done
1. **Git Repository Initialized**
   - Git was initialized in `/Users/leoyu/Downloads/spreadsheet-leocodingg-main/SnakeHandout/`
   - Initial commit made with hash: `751f56b`
   - This commit contains the ORIGINAL unchanged code
   - To revert to original: `git checkout 751f56b`

2. **Project Structure Analyzed**
   - Main GUI code is in `GUI/GUI.Client/`
   - Server DLL files are located at `/Users/leoyu/Desktop/serv/`
   - The game uses MySQL with hardcoded credentials in the code

### Key Files and Their Purpose
1. **NetworkController.cs** - Handles TCP connection to game server AND MySQL database
   - Hardcoded connection string: `server=atr.eng.utah.edu;database=u1459474;uid=u1459474;password=Leospassword;`
   - Manages game state synchronization
   - Records game statistics to database

2. **SnakeGUI.razor** - Main game UI component
   - Currently expects a server connection
   - Uses NetworkController for all game logic

3. **Server files in /Desktop/serv/**
   - Contains `Server.dll` and dependencies
   - Has `settings.xml` with game configuration (walls, world size, etc.)

## The Problem
The student wants to host this game online for their resume, but:
- Static hosting sites (GitHub Pages, Netlify) can't run TCP servers
- The current version requires both a game server and MySQL database
- Cloud hosting with servers costs money

## Proposed Solution (Not Yet Implemented)
Create a single-player version that runs entirely in the browser by:

1. **Create MockNetworkController.cs**
   - Simulates all server behavior locally
   - No database connections needed
   - Implements game logic (snake movement, collision detection, powerup spawning)
   - Uses the same interface as NetworkController

2. **Modify SnakeGUI.razor**
   - Add a toggle for "Single Player Mode"
   - Use MockNetworkController when in single-player mode
   - Keep multiplayer option for local testing with server

3. **Game Logic to Implement in Mock Controller**
   - Snake movement (5 units per frame)
   - Wall collision detection (using wall data from settings.xml)
   - Self-collision detection
   - Powerup spawning and collection
   - Score tracking
   - Death and respawn mechanics

## Next Steps for Implementation

### Step 1: Create MockNetworkController
```csharp
// Key methods to implement:
- HandleConnection() - Initialize game world with walls
- SendMovementCommand() - Process movement locally
- GameLoop() - Run at 50 FPS (20ms intervals)
- MoveSnake() - Update snake position
- CheckCollisions() - Wall, self, and powerup collisions
- SpawnPowerup() - Random powerup generation
```

### Step 2: Update UI
Add to SnakeGUI.razor:
```razor
<input type="checkbox" @bind="_useSinglePlayer" /> Single Player Mode
```

Modify connection logic to use appropriate controller based on mode.

### Step 3: Test Locally
1. Run `dotnet build` in the GUI folder
2. Run `dotnet run` 
3. Test single-player mode works without server

### Step 4: Deploy
For static hosting:
1. Publish as Blazor WebAssembly (not Server)
2. Deploy to GitHub Pages, Netlify, or Vercel

## Important Notes
- The WebServer project in the solution is for viewing game statistics, not running the game
- The actual game server is the compiled DLL in /Desktop/serv/
- All game settings (walls, world size) are in `/Desktop/serv/settings.xml`

## Commands for Next Session
```bash
# Navigate to project
cd /Users/leoyu/Downloads/spreadsheet-leocodingg-main/SnakeHandout

# Check git status
git status

# See what's been changed
git diff

# Revert to original if needed
git checkout 751f56b -- .

# Continue from last changes
git log --oneline
```

## Architecture Summary
```
Current Architecture:
Client (Blazor) → TCP → Game Server → Game Logic
       ↓                      ↓
    MySQL DB ←────────────────┘

Proposed Architecture for Web Hosting:
Client (Blazor) → MockNetworkController → Local Game Logic
```

This approach keeps the original multiplayer code intact while adding a self-contained single-player mode suitable for portfolio hosting.