using System.Diagnostics;
using System.Net.NetworkInformation;
using Part1_Threads.Models;
using Part1_Threads.Tools;

namespace Part1_Threads
{
    public class Program
    {
        private const int NUMBER_OF_KINGS = 5;
        private const int NUMBER_OF_CHOPSTICKS = 5;
        private const int MAX_HUNGRY_TIME = 15000;
        private const int MAX_TIME = 30000;
        private static readonly ConsoleColor[] kingsColors = { ConsoleColor.Red, ConsoleColor.White, ConsoleColor.Blue, ConsoleColor.Magenta, ConsoleColor.Black };
        private static bool run = true;

        public static void Main(string[] args)
        {
            King[] kings = new King[NUMBER_OF_KINGS];
            Chopstick[] chopsticks = new Chopstick[NUMBER_OF_CHOPSTICKS];

            for (int i = 0; i < NUMBER_OF_CHOPSTICKS; i++)
            {
                chopsticks[i] = new Chopstick { Id = i };
            }

            for (int i = 0; i < NUMBER_OF_KINGS; i++)
            {
                int id = i;
                int leftChopstickId = id;
                int rightChopstickId = id + 1 >= NUMBER_OF_CHOPSTICKS ? 0 : id + 1;

                Thread king = new Thread(() =>
                {
                    King king = new King(id, leftChopstickId, rightChopstickId, kingsColors[id >= kingsColors.Length ? 0 : id]);
                    kings[id] = king;

                    while (run)
                    {
                        king.Think();

                        king.RequestTime = DinnerHelper.GetCurrentTime();

                        king.RequestLeftChopstick();
                        lock (chopsticks[leftChopstickId])
                        {
                            king.RequestRightChopstick();
                            lock (chopsticks[rightChopstickId])
                            {
                                king.Eat();

                                if (king.MaxHungryTime > MAX_HUNGRY_TIME)
                                {
                                    run = false;
                                }

                                king.ReleaseChopsticks();
                            }
                        }
                    }
                });
                king.Start();
            }

            ProgramTimer();

            Thread.Sleep(4000);

            DinnerHelper.WriteStaticsConsole(kings);
            DinnerHelper.WriteStaticsFile(kings);
        }

        private static void ProgramTimer()
        {
            int milliseconds = 0;

            while (run)
            {
                Thread.Sleep(1000);
                milliseconds += 1000;

                if (milliseconds >= MAX_TIME)
                {
                    run = false;
                }
            }
        }
    }
}
