namespace MoBot.Structure
{
    public class MathHelper
    {
        public static int floor_double(double p)
        {
            var i = (int)p;
            return i;
        }

        public static int floor_float(float p)
        {
            var i = (int) p;
            return i < 0 ? i - 1 : i;
        }
    }
}
