// <authors> [Leo Yu] [Ethan Edwards] </authors>
// <date> [April 11, 2025] </date>

namespace GUI.Client.Models
{
    /// <summary>
    /// Represents a 2D point or vector with integer coordinates.
    /// </summary>
    public class Point2D
    {
        /// <summary>
        /// Gets or sets the X-coordinate of this point.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y-coordinate of this point.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point2D"/> class with X=0, Y=0.
        /// Required for JSON deserialization.
        /// </summary>
        public Point2D()
        {
            X = 0;
            Y = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point2D"/> class with the specified coordinates.
        /// </summary>
        /// <param name="x">The X-coordinate.</param>
        /// <param name="y">The Y-coordinate.</param>
        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}