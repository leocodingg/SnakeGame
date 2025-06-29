// <authors> [Leo Yu] [Ethan Edwards] </authors>
// <date> [April 11, 2025] </date>

using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    /// <summary>
    /// Represents a single wall in the world, described by two endpoints 
    /// (which may define a horizontal or vertical segment).
    /// </summary>
    public class Walls
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Walls"/> class for JSON deserialization.
        /// </summary>
        public Walls()
        {
            endPoint1 = new Point2D();
            endPoint2 = new Point2D();
        }

        /// <summary>
        /// Gets or sets the unique ID of the wall 
        /// </summary>
        [JsonPropertyName("wall")]
        public int wallID { get; set; }

        /// <summary>
        /// Gets or sets the first endpoint of this wall.
        /// </summary>
        [JsonPropertyName("p1")]
        public Point2D endPoint1 { get; set; }

        /// <summary>
        /// Gets or sets the second endpoint of this wall.
        /// </summary>
        [JsonPropertyName("p2")]
        public Point2D endPoint2 { get; set; }
    }
}