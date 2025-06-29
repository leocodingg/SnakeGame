// <authors> [Leo Yu] [Ethan Edwards] </authors>
// <date> [April 11, 2025] </date>

using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    /// <summary>
    /// Represents a snake controlled by a player, including its body coordinates, 
    /// score, and various status flags.
    /// </summary>
    public class Snake
    {
        /// <summary>
        /// Unique ID for this snake (assigned by the server).
        /// </summary>
        [JsonPropertyName("snake")]
        [JsonInclude]
        public int snakeID;
        
        /// <summary>
        /// Unique ID for this snake (assigned by the server).
        /// </summary>
        public string name { get; set; } 
        
        /// <summary>
        /// A list of points representing the snake's body. Each pair of consecutive
        /// points forms a segment, with the last point typically being the snake's head.
        /// </summary>
        public List<Point2D> body { get; set; }

        /// <summary>
        /// An axis-aligned vector representing the snake's direction 
        /// (purely horizontal or vertical).
        /// </summary>
        [JsonPropertyName("dir")]
        public Point2D snakeDirection { get; set; }

        /// <summary>
        /// The player's current score (number of powerups collected).
        /// </summary>
        public int score { get; set; }

        /// <summary>
        /// True only on the exact frame in which this snake died.
        /// Useful for rendering death animations or explosions.
        /// </summary>
        [JsonInclude]
        public bool died { get; set; }

        /// <summary>
        /// Indicates whether the snake is currently alive (true) or is waiting to respawn (false).
        /// </summary>
        public bool alive { get; set; }

        /// <summary>
        /// Indicates if the player controlling this snake disconnected 
        /// on this frame.
        /// </summary>
        [JsonPropertyName("dc")]
        public bool disconnected { get; set; }

        /// <summary>
        /// True only for the single frame that the player joins. 
        /// </summary>
        public bool join { get; set; }

        /// <summary>
        /// Keeps track of high score for DB logic.
        /// </summary>
        [JsonIgnore]
        public int maxScore { get; set; } = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Snake"/> class. 
        /// Required for JSON deserialization.
        /// </summary>
        public Snake()
        {
            body = new List<Point2D>();
            snakeDirection = new Point2D();
        }
    }
}
