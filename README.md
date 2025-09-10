# 🐍 Multiplayer Snake Game

A real-time multiplayer Snake game built with .NET and Blazor WebAssembly, featuring AWS cloud deployment capabilities.

## 🎮 Features

- **Real-time Multiplayer**: Multiple players can compete simultaneously
- **Client-Server Architecture**: Dedicated game server handling multiple client connections
- **Web-based Client**: Blazor WebAssembly frontend - no installation required
- **Statistics Dashboard**: Track game sessions, scores, and player performance
- **Database Integration**: MySQL database for persistent game statistics
- **AWS Deployment Ready**: Configured for cloud hosting on AWS infrastructure

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────┐
│                     Internet Users                       │
└─────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────┐
│              CloudFront (CDN) → S3 Bucket               │
│                  (Blazor WebAssembly Client)            │
└─────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────┐
│                    EC2 Instance                          │
│  ┌─────────────────────────────────────────────────┐   │
│  │  Game Server (Port 11000) - Server.dll          │   │
│  │  • Handles game logic                           │   │
│  │  • Manages player connections                   │   │
│  │  • Updates game state                           │   │
│  └─────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────┐   │
│  │  Web Server (Port 80) - Statistics Dashboard    │   │
│  │  • Displays game statistics                     │   │
│  │  • Shows leaderboards                           │   │
│  └─────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────┐
│                    RDS MySQL Database                    │
│              • Stores game sessions                      │
│              • Tracks player scores                      │
│              • Maintains leaderboards                    │
└─────────────────────────────────────────────────────────┘
```

## 📁 Project Structure

```
SnakeHandout/
├── GUI/                      # Client applications
│   ├── GUI/                  # Main Blazor WebAssembly client
│   └── GUI.Client/           # Client-side logic
├── WebServer/                # Statistics web server
├── serv/                     # Server binaries and dependencies
│   ├── Server.dll            # Main game server
│   ├── DatabaseController.dll # Database operations
│   ├── Networking.dll        # Network communication
│   ├── World.dll             # Game world logic
│   └── settings.xml          # Server configuration
├── setup_database.sql        # Database schema
└── CLAUDE.md                 # AWS deployment documentation
```

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- MySQL Server (local or cloud)
- AWS Account (for cloud deployment)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/leocodingg/SnakeGame.git
   cd SnakeGame
   ```

2. **Set up the database**
   ```bash
   mysql -u root -p < setup_database.sql
   ```

3. **Update connection strings**
   - Modify database connection strings in `NetworkController.cs` and `WebServer/Program.cs`

4. **Run the game server**
   ```bash
   cd serv
   dotnet Server.dll
   ```

5. **Run the web server** (in a new terminal)
   ```bash
   cd WebServer
   dotnet run
   ```

6. **Run the client** (in a new terminal)
   ```bash
   cd GUI/GUI
   dotnet run
   ```

7. **Access the game**
   - Game client: `http://localhost:5000`
   - Statistics dashboard: `http://localhost:80`

## ☁️ AWS Deployment

### Infrastructure Setup

1. **RDS MySQL Database**
   - Create an RDS MySQL instance
   - Configure security groups for port 3306
   - Run `setup_database.sql` to create schema

2. **EC2 Instance**
   - Launch a t2.micro instance (free tier eligible)
   - Install .NET 8 runtime
   - Configure security groups:
     - Port 22 (SSH)
     - Port 80 (HTTP)
     - Port 11000 (Game Server)

3. **S3 & CloudFront**
   - Create S3 bucket for static hosting
   - Configure CloudFront distribution
   - Deploy Blazor WebAssembly client

### Deployment Steps

```bash
# SSH to EC2
ssh -i your-key.pem ec2-user@your-ec2-ip

# Copy server files
scp -i your-key.pem -r serv/ ec2-user@your-ec2-ip:~/

# Run server on EC2
dotnet ~/serv/Server.dll

# Deploy client to S3
dotnet publish GUI/GUI -c Release
aws s3 sync GUI/GUI/bin/Release/net8.0/publish/wwwroot s3://your-bucket/
```

## 🎯 Game Rules

- Control your snake using arrow keys or WASD
- Collect food to grow and increase your score
- Avoid colliding with walls and other snakes
- Last snake standing wins!

## 🛠️ Technologies Used

- **Backend**: .NET 8, C#
- **Frontend**: Blazor WebAssembly
- **Database**: MySQL
- **Networking**: TCP Sockets
- **Cloud**: AWS (EC2, RDS, S3, CloudFront)

## 📊 Database Schema

### Games Table
- `GameID`: Unique game identifier
- `StartTime`: When the game started
- `EndTime`: When the game ended

### Players Table
- `PlayerID`: Unique player identifier
- `PlayerName`: Player's display name
- `MaxScore`: Highest score achieved
- `EnterTime`: When player joined
- `LeaveTime`: When player left
- `GameID`: Associated game

## 🔧 Configuration

Server settings can be modified in `serv/settings.xml`:
- Game speed
- World size
- Maximum players
- Food spawn rate

## 📝 Known Issues

- ARM64 vs x86_64 architecture compatibility when deploying to AWS
- RDS connection may require VPC and security group configuration
- Ensure .NET 8 runtime is installed on deployment targets

## 🤝 Contributing

This is a university project for learning purposes. Feel free to fork and experiment!

## 📚 Learning Outcomes

This project demonstrates:
- Client-server architecture design
- Real-time networking with TCP
- Database integration
- Cloud deployment strategies
- Cross-platform development with .NET
- Modern web development with Blazor

## 📄 License

This project is part of university coursework and is for educational purposes.

## 🙏 Acknowledgments

- Course instructor for providing the initial codebase
- AWS for free tier resources
- .NET community for excellent documentation

---

*For detailed AWS deployment instructions and troubleshooting, see [CLAUDE.md](CLAUDE.md)*