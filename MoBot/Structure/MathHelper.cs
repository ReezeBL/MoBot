using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure
{
    class MathHelper
    {
        public static int floor_double(double p_76128_0_)
        {
            int i = (int)p_76128_0_;
            return p_76128_0_ < (double)i ? i : i;
        }
    }
}
