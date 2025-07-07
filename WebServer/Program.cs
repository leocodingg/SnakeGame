// WebServer.cs – super‑simple version (no StringBuilder, no byte arrays)
// <author> Leo Yu </author>
// <date> April 21, 2025 </date>

using System.Text;
using MySql.Data.MySqlClient;
using GUI.Client.Controllers;

namespace WebServer;

public static class WebServer
{
    /// <summary>
    /// MySQL connection string used to connect to the game database.
    /// </summary>
    private const string ConnStr = "server=snakegame-db.clmai4aqc81w.ap-southeast-2.rds.amazonaws.com;database=snakegame;uid=admin;password=snakegame;";

    /// <summary>
    /// Entry point for the web server. Starts the server and listens for incoming connections.
    /// </summary>
    public static void Main()
    {
        Server.StartServer(HandleClient, 80);
        Console.ReadLine(); // keep window open
    }

    // --------- REQUEST HANDLER ---------
    
    /// <summary>
    /// Handles a single client connection by parsing the request and sending the appropriate response.
    /// </summary>
    private static void HandleClient(NetworkConnection conn)
    {
        string? firstLine = conn.ReadLine();   
        if (firstLine == null)
        {
            conn.Disconnect();
            return;
        }

        if (firstLine.StartsWith("GET / "))
        {
            string html = "<html><h3>Welcome to the Snake Games Database!</h3><a href='/games'>View Games</a></html>";
            SendHtml(conn, html);
        }
        else if (firstLine.StartsWith("GET /games?gid="))
        {
            int gid = ParseGid(firstLine);
            if (gid < 0)
                SendNotFound(conn);
            else
                ServeGame(conn, gid);
        }
        else if (firstLine.StartsWith("GET /games"))
        {
            ServeAllGames(conn);
        }
        else
        {
            SendNotFound(conn);
        }

        conn.Disconnect();
    }

    // --------- BUILD PAGES ---------
    
    /// <summary>
    /// Serves a table listing all games in the database.
    /// </summary>
    /// <param name="conn">The client's network connection.</param>
    private static void ServeAllGames(NetworkConnection conn)
    {
        string html = "<html><table border='1'><tr><td>ID</td><td>Start</td><td>End</td></tr>";

        using (MySqlConnection db = new MySqlConnection(ConnStr))
        {
            db.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT GameID, StartTime, EndTime FROM Games", db);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    int id = rdr.GetInt32(0);
                    string start = rdr.GetDateTime(1).ToString();
                    string end   = rdr.IsDBNull(2) ? "Running" : rdr.GetDateTime(2).ToString();
                    html += $"<tr><td><a href='/games?gid={id}'>{id}</a></td><td>{start}</td><td>{end}</td></tr>";
                }
            }
        }

        html += "</table></html>";
        SendHtml(conn, html);
    }

    
    /// <summary>
    /// Serves detailed information for a specific game, including all players and their scores.
    /// </summary>
    /// <param name="conn">The client's network connection.</param>
    /// <param name="gid">The game ID to look up.</param>
    private static void ServeGame(NetworkConnection conn, int gid)
    {
        string html = $"<html><h3>Stats for Game {gid}</h3><table border='1'><tr><td>Player ID</td><td>Name</td><td>Max Score</td><td>Enter</td><td>Leave</td></tr>";

        using (MySqlConnection db = new MySqlConnection(ConnStr))
        {
            db.Open();
            MySqlCommand cmd = new MySqlCommand($"SELECT PlayerID, PlayerName, MaxScore, EnterTime, LeaveTime FROM Players WHERE GameID = {gid}", db);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    int pid = rdr.GetInt32(0);
                    string name = rdr.GetString(1);
                    int max  = rdr.GetInt32(2);
                    string enter = rdr.GetDateTime(3).ToString();
                    string leave = rdr.IsDBNull(4) ? "Connected" : rdr.GetDateTime(4).ToString();
                    html += $"<tr><td>{pid}</td><td>{name}</td><td>{max}</td><td>{enter}</td><td>{leave}</td></tr>";
                }
            }
        }

        html += "</table></html>";
        SendHtml(conn, html);
    }

    // --------- RESPONSE HELPERS ---------
    
    /// <summary>
    /// Sends an HTML response to the client with proper HTTP headers.
    /// </summary>
    /// <param name="conn">The client's network connection.</param>
    /// <param name="html">The HTML content to send.</param>
    private static void SendHtml(NetworkConnection conn, string html)
    {
        int len = Encoding.UTF8.GetByteCount(html);
        string header = "HTTP/1.1 200 OK\r\n" +
                        "Connection: close\r\n" +
                        "Content-Type: text/html; charset=UTF-8\r\n" +
                        $"Content-Length: {len}\r\n\r\n";
        conn.Send(header + html);
    }

    /// <summary>
    /// Sends a 404 Not Found HTTP response to the client.
    /// </summary>
    /// <param name="conn">The client's network connection.</param>
    private static void SendNotFound(NetworkConnection conn)
    {
        conn.Send("HTTP/1.1 404 Not Found\r\nConnection: close\r\n\r\n");
    }

    // --------- SMALL UTIL ---------
    
    /// <summary>
    /// Parses the game ID (gid) from the HTTP request line.
    /// </summary>
    /// <param name="line">The HTTP request line containing the query string.</param>
    /// <returns>The parsed game ID, or -1 if parsing fails.</returns>
    private static int ParseGid(string line)
    {
        string[] parts = line.Split('=');
        if (parts.Length < 2) return -1;
        string num = parts[1].Split(' ')[0];
        return int.TryParse(num, out int gid) ? gid : -1;
    }
}
