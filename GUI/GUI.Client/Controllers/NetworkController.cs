// <authors> [Leo Yu] </authors>
// <date> [April 11, 2025] </date>

using System.Text.Json;
using GUI.Client.Controllers;
using GUI.Client.Models;
using MySql.Data.MySqlClient;

namespace GUI.Client
{
    /// <summary>
    /// Handles network communication with the server and database operations for a game session.
    /// </summary>
    public class NetworkController
    {
        /// <summary>
        /// Holds the game world state including snakes, walls, and powerups.
        /// </summary>
        private World _world;
        
        /// <summary>
        /// Manages TCP connection to the game server.
        /// </summary>
        private NetworkConnection _connection = new();
        
        
        /// <summary>
        /// Maintains the active MySQL connection for the duration of the game.
        /// </summary>
        private MySqlConnection? db;          

        /// <summary>
        /// ID assigned to the current player by the server.
        /// </summary>
        private int _playerID;
        
        private string _playerName;
        
        /// <summary>
        /// Indicates whether all walls have been received from the server.
        /// </summary>
        private bool _wallsReceived = false;

        /// <summary>
        /// Connection string used to access the MySQL database.
        /// </summary>
        public const string connectionString =
            "server=atr.eng.utah.edu;database=u1459474;uid=u1459474;password=Leospassword;";

        /// <summary>
        /// ID of the current game session, assigned after inserting the game row.
        /// </summary>
        private int gameID;

        // ------------------------------------------------ HandleConnection
        /// <summary>
        /// Handles the full game connection setup, database logging, and initial world state population.
        /// </summary>
        public void HandleConnection(string host,int port,string name,World world)
        {
            _world = world;

            try
            {
                // 1. start TCP → server
                _connection.Connect(host, port);

                // 2. open ONE MySQL connection for the whole game
                db = new MySqlConnection(connectionString);
                db.Open();

                // 3. insert a new Games row and save the auto‑id
                gameID = InsertGame();

                // 4. send player name, read ID & world size
                _connection.Send(name);

                _playerID = int.Parse(_connection.ReadLine());
                _world.Size = int.Parse(_connection.ReadLine());

                // 5. get walls then live updates
                ReceiveWalls();
                ReceiveUpdates();
            }
            //TODO do I need?
            catch (Exception ex)
            {
                 if (db != null)
                 {
                     SetGameEnd();        // EndTime
                     SetPlayerLeaveAll(); // any players still NULL
            
                     db.Close();
                     db = null;
                 }
             }
            finally   // always run – even on normal quit
            {
                if (db != null)
                {
                    SetGameEnd();        // EndTime
                    SetPlayerLeaveAll(); // any players still NULL

                    db.Close();
                    db = null;
                }
            }
        }

        // ------------------------------------------------ wall & update loops (unchanged)
        
        /// <summary>
        /// Continuously receives wall objects from the server until game object updates begin.
        /// </summary>
        private void ReceiveWalls()
        {
            while (true)
            {
                string wallJson = _connection.ReadLine();
                if (wallJson.Contains("\"wall\":"))
                {
                    Walls wall = JsonSerializer.Deserialize<Walls>(wallJson)
                                 ?? throw new Exception("bad wall json");
                    lock (_world) { _world.AddWall(wall); }
                }
                else { ProcessGameObj(wallJson); _wallsReceived = true; return; }
            }
        }
        
        /// <summary>
        /// Continuously receives game object updates (snakes, powerups) from the server.
        /// </summary>
        private void ReceiveUpdates()
        {
            while (_connection.IsConnected)
            {
                string update = _connection.ReadLine();
                ProcessGameObj(update);
            }
        }

        // ------------------------------------------------ ProcessGameObj (SQL parts refactored)
     
        /// <summary>
        /// Processes a single game object JSON string and updates the world state and database accordingly.
        /// </summary>
        private void ProcessGameObj(string json)
        {
            lock (_world)
            {
                if (json.Contains("\"snake\":"))
                {
                    Snake s = JsonSerializer.Deserialize<Snake>(json) ?? new Snake();
                    if (s.disconnected) SetPlayerLeave(s.snakeID);

                    bool isNew = _world.UpdateSnake(s);
                    if (isNew) InsertPlayer(s);
                    else if (s.score > s.maxScore)
                    {
                        SetPlayerScore(s.snakeID, s.score);
                        s.maxScore = s.score;
                    }
                }
                else if (json.Contains("\"power\":"))
                {
                    Powerup p = JsonSerializer.Deserialize<Powerup>(json) ?? new Powerup();
                    _world.UpdatePower(p);
                }
            }
        }

        // ------------------------------------------------ Movement
       
        /// <summary>
        /// Sends a movement direction command to the server.
        /// </summary>
        public void SendMovementCommand(string dir)
        {
            if (_connection.IsConnected && _wallsReceived)
                _connection.Send(JsonSerializer.Serialize(new { moving = dir }));
        }
        
        // ================================================== tiny SQL helpers
       
        /// <summary>
        /// Inserts a new row into the Games table and returns the auto-generated game ID.
        /// </summary>
        private int InsertGame()
        {
            string ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySqlCommand cmd = db!.CreateCommand();
            cmd.CommandText = $"INSERT INTO Games (StartTime) VALUES ('{ts}');";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "SELECT LAST_INSERT_ID();";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

    
        /// <summary>
        /// Sets the EndTime of the current game in the database.
        /// </summary>
        private void SetGameEnd()
        {
            string ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySqlCommand cmd = db!.CreateCommand();
            cmd.CommandText = $"UPDATE Games SET EndTime = '{ts}' WHERE GameID = {gameID};";
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Inserts a new player record into the Players table.
        /// </summary>
        /// <param name="s">The Snake object representing the player.</param>
        private void InsertPlayer(Snake s)
        {
            string ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySqlCommand cmd = db!.CreateCommand();
            cmd.CommandText =
                $"INSERT INTO Players (PlayerID,PlayerName,MaxScore,EnterTime,GameID) " +
                $"VALUES ({s.snakeID},'{s.name}',{s.score},'{ts}',{gameID});";
            cmd.ExecuteNonQuery();
        }

        
        /// <summary>
        /// Sets the LeaveTime for a specific player in the database.
        /// </summary>
        /// <param name="pid">The player ID.</param>
        private void SetPlayerLeave(int pid)
        {
            string ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySqlCommand cmd = db!.CreateCommand();
            cmd.CommandText =
                $"UPDATE Players SET LeaveTime = '{ts}' WHERE PlayerID = {pid} AND GameID = {gameID};";
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Sets the LeaveTime for all players in the current game session who haven't already left.
        /// </summary>
        private void SetPlayerLeaveAll()
        {
            string ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySqlCommand cmd = db!.CreateCommand();
            cmd.CommandText =
                $"UPDATE Players SET LeaveTime = '{ts}' WHERE GameID = {gameID} AND LeaveTime IS NULL;";
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Updates the MaxScore for a specific player.
        /// </summary>
        /// <param name="pid">The player ID.</param>
        /// <param name="score">The new high score to set.</param>
        private void SetPlayerScore(int pid,int score)
        {
            MySqlCommand cmd = db!.CreateCommand();
            cmd.CommandText =
                $"UPDATE Players SET MaxScore = {score} WHERE PlayerID = {pid} AND GameID = {gameID};";
            cmd.ExecuteNonQuery();
        }
        
        // ======================================== Misc
        
        /// <summary>
        /// Disconnects the client from the server.
        /// </summary>
        public void Disconnect() { _connection.Disconnect(); }
        
        /// <summary>
        /// Gets the ID of the current player.
        /// </summary>
        /// <returns>The player's ID.</returns>
        public int GetPlayerID() => _playerID;

    }
}
