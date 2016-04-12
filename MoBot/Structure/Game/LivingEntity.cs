using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure.Game
{
    class LivingEntity : Entity
    {
        public double x, y, z;
        public float yaw, pitch;
        public float Health;

        public override string ToString()
        {
            return $"Utyped Living Entity ({(int)x} | {(int)y} | {(int)z})";
        }
    }
}
