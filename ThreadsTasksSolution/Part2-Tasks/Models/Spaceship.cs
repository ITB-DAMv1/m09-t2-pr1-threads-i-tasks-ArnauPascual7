using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Part2_Tasks.Models
{
    public class Spaceship
    {
        public Position Position { get; set; }
        public char Sprite { get; set; } = '^';
        public int Points { get; set; } = 0;

        public Spaceship(Position position)
        {
            Position = position;
        }
    }
}
