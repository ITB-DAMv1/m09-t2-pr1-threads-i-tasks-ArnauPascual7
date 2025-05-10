using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Part2_Tasks.Models
{
    public class Asteroid
    {
        public Position Position { get; set; }
        public float Accel { get; set; } = 0.2f;
        public char Sprite { get; set; } = '*';

        public Asteroid(Position position)
        {
            Position = position;
        }
    }
}
