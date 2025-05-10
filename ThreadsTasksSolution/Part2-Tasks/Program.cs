using System.Diagnostics;
using Part2_Tasks.Models;

namespace Part2_Tasks
{
    public class Program
    {
        private const int WINDOW_WIDTH = 40;
        private const int WINDOW_HEIGHT = 20;
        private const int MAX_GAME_TIME = 60000;

        private static bool checkWindowSize = true;
        private static bool gameRunning = true;
        private static bool gameOver = false;
        private static bool gameQuit = false;
        private static bool gameExit = false;

        private static Spaceship spaceship;
        private static List<Asteroid> asteroids = new List<Asteroid>();

        private static Stopwatch gameClock = new Stopwatch();
        private static TimeSpan lastRender;
        private static TimeSpan lastUpdate;

        private static readonly object consoleLock = new object();
        private static readonly object asteroidLock = new object();

        public static async Task Main(string[] args)
        {
            while (!gameExit)
            {
                Console.Title = "Asterioids - Arnau Pascual";
                Console.CursorVisible = false;

                spaceship = new Spaceship(new Position(WINDOW_WIDTH / 2, WINDOW_HEIGHT - 2));

                gameClock.Start();
                lastRender = gameClock.Elapsed;
                lastUpdate = gameClock.Elapsed;

                var consoleTask = Task.Run(ConsoleSize);
                var inputTask = Task.Run(ProcessInput);
                var spawnTask = Task.Run(SpawnAsteroids);
                var updateTask = Task.Run(GameLoop);
                var renderTask = Task.Run(RenderLoop);

                await Task.WhenAll(inputTask, spawnTask, updateTask, renderTask);

                ResultsScreen();
            }
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

                    if (gameClock.ElapsedMilliseconds > MAX_GAME_TIME)
                        gameRunning = false;
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
                case ConsoleKey.A:
                    newPos.X--;
                    break;
                case ConsoleKey.D:
                    newPos.X++;
                    break;
                case ConsoleKey.Q:
                    gameRunning = false;
                    gameQuit = true;
                    break;
            }

            newPos.X = Math.Clamp(newPos.X, 1, WINDOW_WIDTH - 2);
            spaceship.Position = newPos;
        }

        private static async Task SpawnAsteroids()
        {
            while (gameRunning)
            {
                var x = Random.Shared.Next(1, WINDOW_WIDTH - 2);
                var y = 0;

                lock (asteroidLock)
                {
                    asteroids.Add(new Asteroid(new Position(x, y)));
                }

                await Task.Delay(Random.Shared.Next(100, 1000));
            }
        }

        private static void UpdateGame(TimeSpan deltaTime)
        {
            lock (asteroidLock)
            {
                foreach (var asteroid in asteroids)
                {
                    if (asteroid != null)
                    {
                        Position newPos = asteroid.Position;
                        float accel = asteroid.Accel;

                        accel += 0.2f;

                        newPos.Y += (int)accel;

                        if (accel >= 1) accel = 0.2f;

                        asteroid.Position = newPos;
                        asteroid.Accel = accel;

                        if (spaceship.Position.Y == asteroid.Position.Y)
                        {
                            if (spaceship.Position.X == asteroid.Position.X)
                            {
                                gameRunning = false;
                                gameOver = true;
                            }
                            else if ((spaceship.Position.X == asteroid.Position.X - 1 || spaceship.Position.X == asteroid.Position.X + 1) && asteroid.Accel == 0.2f)
                            {
                                Console.Beep(300, 100);
                                spaceship.Points++;
                            }
                        }

                        if (asteroid.Position.Y >= WINDOW_HEIGHT)
                        {
                            spaceship.Points++;
                        }
                    }
                }
                asteroids.RemoveAll(a => a.Position.Y >= WINDOW_HEIGHT);
            }
        }

        private static void RenderGame()
        {
            Console.SetCursorPosition(0, 0);
            Console.Clear();
            
            WriteSprite(spaceship.Sprite, spaceship.Position);

            lock (asteroidLock)
            {
                foreach (var asteroid in asteroids)
                {
                    WriteSprite(asteroid.Sprite, asteroid.Position);
                }
            }
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

        public static void ResultsScreen()
        {
            lock (consoleLock)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);

                if (gameOver)
                    Console.WriteLine("Has Perdut!");
                else
                {
                    if (gameQuit)
                        Console.WriteLine("Has Sortit!");
                    else
                        Console.WriteLine("Has Guanyat!");
                }

                Console.WriteLine($"\nAsteroides Esquivats: {spaceship.Points}");
                Console.WriteLine($"\nTemps de Joc: {gameClock.ElapsedMilliseconds / 1000} segons");
                Console.WriteLine("\nPrem qualsevol tecla per sortir.");
                Console.WriteLine("\nPrem Enter per a tornar a jugar");

                var key = Console.ReadKey(true).Key;
                Console.Clear();

                if (key == ConsoleKey.Enter)
                {
                    gameRunning = true;
                    gameOver = false;
                    gameQuit = false;
                    spaceship.Points = 0;
                    asteroids.Clear();
                    gameClock.Restart();
                }
                else
                {
                    gameExit = true;
                    Console.WriteLine("Adeu");
                }
            }
        }
    }
}
