using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Part1_Threads.Tools;

namespace Part1_Threads.Models
{
    public class King
    {
        private const int MIN_THINK_TIME = 500;
        private const int MAX_THINK_TIME = 2000;
        private const int MIN_EAT_TIME = 500;
        private const int MAX_EAT_TIME = 1000;

        public int Id { get; set; }
        public int LeftChopstickId { get; private set; }
        public int RightChopstickId { get; private set; }
        public ConsoleColor Color { get; set; }

        public TimeOnly RequestTime { get; set; }
        public TimeOnly LastEatTime { get; set; }
        public int MaxHungryTime { get; set; }
        public int TotalHungryTime { get; set; } = 0;
        public int EatTimes { get; set; } = 0;

        public King(int id, int leftChopstickId, int rightChopstickId, ConsoleColor color)
        {
            Id = id;
            LeftChopstickId = leftChopstickId;
            RightChopstickId = rightChopstickId;
            Color = color;
        }

        public void Think()
        {
            DinnerHelper.WriteState("està pensant", Id, Color, ConsoleColor.DarkCyan);
            Thread.Sleep(new Random().Next(MIN_THINK_TIME, MAX_THINK_TIME));
        }

        public void RequestLeftChopstick()
        {
            DinnerHelper.WriteState($"demana el palet esquerre ({LeftChopstickId})", Id, Color, ConsoleColor.DarkGreen);
        }

        public void RequestRightChopstick()
        {
            DinnerHelper.WriteState($"demana el palet dret ({RightChopstickId})", Id, Color, ConsoleColor.DarkGreen);
        }

        public void Eat()
        {
            EatTimes++;

            LastEatTime = DinnerHelper.GetCurrentTime();

            int hungryTime = (LastEatTime - RequestTime).Milliseconds;
            TotalHungryTime += hungryTime;

            if (hungryTime > MaxHungryTime)
            {
                MaxHungryTime = LastEatTime.Millisecond;
            }

            DinnerHelper.WriteState($"està menjant ({LeftChopstickId}, {RightChopstickId})", Id, Color, ConsoleColor.DarkYellow);
            Thread.Sleep(new Random().Next(MIN_EAT_TIME, MAX_EAT_TIME));
        }

        public void ReleaseChopsticks()
        {
            DinnerHelper.WriteState($"ha deixat els palets ({LeftChopstickId}, {RightChopstickId})", Id, Color, ConsoleColor.DarkRed);
        }
    }
}
