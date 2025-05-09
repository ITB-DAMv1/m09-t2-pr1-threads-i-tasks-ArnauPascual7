using Part2_Tasks.Models;

namespace Part2_Tasks
{
    public class Program
    {
        private const int WINDOW_WIDTH = 40;
        private const int WINDOW_HEIGHT = 20;
        private static bool checkWindowSize = true;
        private static bool gameRunning = true;
        private static Spaceship spaceship;

        public static void Main(string[] args)
        {
            Console.Title = "Asterioids - Arnau Pascual";
            Console.CursorVisible = false;

            spaceship = new Spaceship(new Position(WINDOW_WIDTH / 2, WINDOW_HEIGHT - 2));
            WriteSprite(spaceship.Sprite, spaceship.Position);

            Task.Run(ConsoleSize);
            Task.Run(SpaceshipMovement);

            while (gameRunning) { }
        }

        public static async Task ConsoleSize()
        {
            while (checkWindowSize)
            {
                if (Console.WindowWidth != WINDOW_WIDTH || Console.WindowHeight != WINDOW_HEIGHT)
                {
                    Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEIGHT);
                    Console.SetBufferSize(WINDOW_WIDTH, WINDOW_HEIGHT);
                }
                await Task.Delay(100);
            }
        }

        public static async Task SpaceshipMovement()
        {
            Position position = spaceship.Position;

            while (gameRunning)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.A:
                        position.X--;
                        break;
                    case ConsoleKey.D:
                        position.X++;
                        break;
                }

                if (position.X < 1)
                    position.X = 1;
                else if (position.X >= WINDOW_WIDTH)
                    position.X = WINDOW_WIDTH - 1;

                WriteSprite(spaceship.Sprite, spaceship.Position, position);

                spaceship.Position = position;
            }
        }

        public static void WriteSprite(char sprite, Position oldPosition, Position newPosition)
        {
            CleanSprite(oldPosition);
            WriteSprite(sprite, newPosition);
        }

        public static void WriteSprite(char sprite, Position position)
        {
            Console.SetCursorPosition(position.X, position.Y);
            Console.Write(sprite);
        }
        public static void CleanSprite(Position position)
        {
            Console.SetCursorPosition(position.X, position.Y);
            Console.Write(' ');
        }
    }
}
