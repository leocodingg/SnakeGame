-- Snake Game Database Setup Script
-- Run this on your AWS RDS MySQL instance

-- Create the database
CREATE DATABASE IF NOT EXISTS snakegame;
USE snakegame;

-- Create Games table
CREATE TABLE IF NOT EXISTS Games (
    GameID INT AUTO_INCREMENT PRIMARY KEY,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NULL
);

-- Create Players table
CREATE TABLE IF NOT EXISTS Players (
    PlayerID INT NOT NULL,
    PlayerName VARCHAR(255) NOT NULL,
    MaxScore INT NOT NULL DEFAULT 0,
    EnterTime DATETIME NOT NULL,
    LeaveTime DATETIME NULL,
    GameID INT NOT NULL,
    FOREIGN KEY (GameID) REFERENCES Games(GameID) ON DELETE CASCADE,
    INDEX idx_game_id (GameID),
    INDEX idx_player_game (PlayerID, GameID)
);

-- Verify tables were created
SHOW TABLES;
DESCRIBE Games;
DESCRIBE Players;