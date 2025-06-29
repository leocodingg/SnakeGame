// <authors> [Leo Yu] [Ethan Edwards] </authors>
// <date> [April 11, 2025] </date>

using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    /// <summary>
    /// Represents a powerup object in the game world, which can be collected by a snake.
    /// </summary>
    public class Powerup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Powerup"/> class for JSON deserialization.
        /// </summary>
        public Powerup()
        {
            location = new Point2D();
        }

        /// <summary>
        /// Gets or sets the unique ID of this powerup (as provided by the server).
        /// </summary>
        [JsonPropertyName("power")]
        public int powerID { get; set; }

        /// <summary>
        /// Gets or sets the location of the powerup in the world.
        /// </summary>
        [JsonPropertyName("loc")]
        public Point2D location { get; set; }

        /// <summary>
        /// Indicates whether this powerup has been collected (died) and should be removed.
        /// </summary>
        public bool died { get; set; }
    }
}