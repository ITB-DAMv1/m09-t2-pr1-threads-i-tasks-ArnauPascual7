namespace Part2_Tasks
{
    public class Program
    {
        private const int WINDOW_WIDTH = 100;
        private const int WINDOW_HEIGHT = 30;
        private static bool checkWindowSize = true;

        public static void Main(string[] args)
        {
            Console.Title = "Asterioids - Arnau Pascual";
            Console.CursorVisible = false;

            Task.Run(ConsoleSize);

            Console.ReadKey(true);
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
    }
}
