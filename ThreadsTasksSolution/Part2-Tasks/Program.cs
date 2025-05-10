using System.Diagnostics;
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

        private static Stopwatch gameClock = new Stopwatch();
        private static TimeSpan lastRender;
        private static TimeSpan lastUpdate;
        private static readonly object consoleLock = new object();

        public static async Task Main(string[] args)
        {
            Console.Title = "Asterioids - Arnau Pascual";
            Console.CursorVisible = false;

            spaceship = new Spaceship(new Position(WINDOW_WIDTH / 2, WINDOW_HEIGHT - 2));

            gameClock.Start();
            lastRender = gameClock.Elapsed;
            lastUpdate = gameClock.Elapsed;

            var consoleTask = Task.Run(ConsoleSize);
            var inputTask = Task.Run(ProcessInput);
            var updateTask = Task.Run(GameLoop);
            var renderTask = Task.Run(RenderLoop);

            await Task.WhenAll(inputTask, updateTask, renderTask);
        }

        private static async Task GameLoop()
        {
            const double updateInterval = 1000 / 50.0;

            while (gameRunning)
            {
                var currentTime = gameClock.Elapsed;
                var deltaTime = currentTime - lastUpdate;

                if (deltaTime.TotalMilliseconds >= updateInterval)
                {
                    UpdateGame(deltaTime);
                    lastUpdate = currentTime;
                }

                await Task.Delay(1);
            }
        }

        private static async Task RenderLoop()
        {
            const double renderInterval = 1000 / 20.0;

            while (gameRunning)
            {
                var currentTime = gameClock.Elapsed;
                var deltaTime = currentTime - lastRender;

                if (deltaTime.TotalMilliseconds >= renderInterval)
                {
                    lock (consoleLock)
                    {
                        RenderGame();
                    }
                    lastRender = currentTime;
                }

                await Task.Delay(1);
            }
        }

        private static async Task ProcessInput()
        {
            while (gameRunning)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    HandleInput(key);
                }
                await Task.Delay(10);
            }
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

        private static void HandleInput(ConsoleKey key)
        {
            Position newPos = spaceship.Position;

            switch (key)
            {
                case ConsoleKey.A: newPos.X--; break;
                case ConsoleKey.D: newPos.X++; break;
                case ConsoleKey.Escape: gameRunning = false; break;
            }

            newPos.X = Math.Clamp(newPos.X, 1, WINDOW_WIDTH - 2);
            spaceship.Position = newPos;
        }

        private static void UpdateGame(TimeSpan deltaTime)
        {
            // Logic
        }

        private static void RenderGame()
        {
            Console.SetCursorPosition(0, 0);
            Console.Clear();
            WriteSprite(spaceship.Sprite, spaceship.Position);
        }

        public static void WriteSprite(char sprite, Position position)
        {
            if (position.X >= 0 && position.X < WINDOW_WIDTH &&
                position.Y >= 0 && position.Y < WINDOW_HEIGHT)
            {
                Console.SetCursorPosition(position.X, position.Y);
                Console.Write(sprite);
            }
        }
    }
}
