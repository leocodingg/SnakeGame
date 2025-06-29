// <authors> [Leo Yu] [Ethan Edwards] </authors>
// <date> [April 11, 2025] </date>

namespace GUI.Client.Models
{
    /// <summary>
    /// A static class representing a single global explosion effect. 
    /// If you need multiple simultaneous explosions, consider a non-static approach.
    /// </summary>
    public static class SnakeExplosion
    {
        /// <summary>
        /// Number of frames the explosion should persist.
        /// </summary>
        public static int ExplosionTimer { get; set; }

        /// <summary>
        /// Current explosion radius, which can expand each frame to animate the effect.
        /// </summary>
        public static double ExplosionRadius { get; set; }

        /// <summary>
        /// The fixed location at which the explosion appears (e.g., the snake's head at death).
        /// </summary>
        public static Point2D ExplosionCenter { get; set; }
    }
}