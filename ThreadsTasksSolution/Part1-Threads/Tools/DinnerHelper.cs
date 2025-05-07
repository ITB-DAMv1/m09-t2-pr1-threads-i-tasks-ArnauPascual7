using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Part1_Threads.Models;

namespace Part1_Threads.Tools
{
    public class DinnerHelper
    {
        private readonly static object _lock = new object();
        public static void WriteState(string text, int id, ConsoleColor idColor, ConsoleColor backgroudColor)
        {
            // Result -> Time - El comensal Id Text
            lock (_lock)
            {
                Console.ResetColor();
                Console.BackgroundColor = backgroudColor;
                Console.Write($"{GetCurrentTime()} - El comensal ");
                Console.ForegroundColor = idColor;
                Console.Write(id);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" {text}");
                Console.WriteLine();
                Console.ResetColor();
            }
        }

        public static TimeOnly GetCurrentTime()
        {
            return TimeOnly.FromDateTime(DateTime.Now);
        }

        public static void WriteStaticsFile(King[] kings)
        {
            const string FileName = "statics.csv";
            const string FilePath = @"..\..\..\Files\" + FileName;

            if (File.Exists(FilePath))
            {
                using (StreamWriter sw = File.CreateText(FilePath))
                {
                    sw.WriteLine("Id,MaxHungryTime,EatTimes");
                    foreach (King king in kings)
                    {
                        sw.WriteLine($"{king.Id},{king.MaxHungryTime},{king.EatTimes}");
                    }
                }
            }
            else
            {
                Debug.WriteLine("?: File does not exist -> " + Path.GetFullPath(FilePath));
            }
        }
    }
}
