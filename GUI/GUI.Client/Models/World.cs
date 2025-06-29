// <authors> [Leo Yu] [Ethan Edwards] </authors>
// <date> [April 11, 2025] </date>

namespace GUI.Client.Models
{
    /// <summary>
    /// Represents the entire game world, including snakes, walls, and powerups.
    /// Stores the world size and provides methods to update and retrieve game objects.
    /// </summary>
    public class World
    {
        /// <summary>
        /// Dictionary storing powerups keyed by their unique ID.
        /// </summary>
        private readonly Dictionary<int, Powerup> power;

        /// <summary>
        /// Dictionary storing snakes keyed by their unique ID.
        /// </summary>
        private readonly Dictionary<int, Snake> snakes;

        /// <summary>
        /// Dictionary storing walls keyed by their unique ID.
        /// </summary>
        private readonly Dictionary<int, Walls> walls;

        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class with a given size.
        /// </summary>
        /// <param name="size">The size of one side of the square world.</param>
        public World(int size)
        {
            power = new Dictionary<int, Powerup>();
            snakes = new Dictionary<int, Snake>();
            walls = new Dictionary<int, Walls>();
            Size = size;
        }

        /// <summary>
        /// Initializes a shallow copy of the specified world.
        /// Copies references to snakes, walls, and powerup dictionaries but not deeply.
        /// </summary>
        /// <param name="world">The existing world to copy.</param>
        public World(World world)
        {
            power = new Dictionary<int, Powerup>(world.power);
            snakes = new Dictionary<int, Snake>(world.snakes);
            walls = new Dictionary<int, Walls>(world.walls);
            Size = world.Size;
        }

        /// <summary>
        /// Gets or sets the size (width/height) of this square world.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets the width of the world. Equivalent to <see cref="Size"/>.
        /// </summary>
        public int Width => Size;

        /// <summary>
        /// Gets the height of the world. Equivalent to <see cref="Size"/>.
        /// </summary>
        public int Height => Size;

        /// <summary>
        /// Adds or updates the specified wall in the world dictionary.
        /// </summary>
        /// <param name="wall">The wall object to add or update.</param>
        public void AddWall(Walls wall)
        {
            walls[wall.wallID] = wall;
        }

        /// <summary>
        /// Adds or updates the specified snake in the world dictionary.
        /// Removes it if the snake is marked as disconnected.
        /// </summary>
        /// <returns>True if the snake is new for easy database max score logic.</returns>
        public bool UpdateSnake(Snake snake)
        {
            if (snake.disconnected)
            {
                // Remove disconnected snakes
                snakes.Remove(snake.snakeID);
                return false;
            }

            bool isNew = !snakes.ContainsKey(snake.snakeID);
            snakes[snake.snakeID] = snake;
            return isNew;
        }

        /// <summary>
        /// Adds or updates the specified powerup in the world dictionary.
        /// Removes it if the powerup is marked as died.
        /// </summary>
        /// <param name="powerup">The powerup object to add or update.</param>
        public void UpdatePower(Powerup powerup)
        {
            if (powerup.died)
            {
                // Remove eaten power-ups
                power.Remove(powerup.powerID);
                return;
            }

            power[powerup.powerID] = powerup;
        }

        /// <summary>
        /// Returns an enumeration of all snakes currently in the world.
        /// </summary>
        /// <returns>A collection of <see cref="Snake"/> objects.</returns>
        public IEnumerable<Snake> GetSnakes()
        {
            return snakes.Values;
        }

        /// <summary>
        /// Returns an enumeration of all powerups currently in the world.
        /// </summary>
        /// <returns>A collection of <see cref="Powerup"/> objects.</returns>
        public IEnumerable<Powerup> GetFoods()
        {
            return power.Values;
        }

        /// <summary>
        /// Returns an enumeration of all walls currently in the world.
        /// </summary>
        /// <returns>A collection of <see cref="Walls"/> objects.</returns>
        public IEnumerable<Walls> GetWalls()
        {
            return walls.Values;
        }

        /// <summary>
        /// Retrieves the snake with the specified ID, or null if not found.
        /// </summary>
        /// <param name="id">The unique ID of the snake.</param>
        /// <returns>The <see cref="Snake"/> object, or null if not found.</returns>
        public Snake GetSnakeById(int id)
        {
            return snakes.TryGetValue(id, out Snake snake) ? snake : null;
        }
    }
}
