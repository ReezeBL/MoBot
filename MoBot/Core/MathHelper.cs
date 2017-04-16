namespace MoBot.Core
{
    public class MathHelper
    {
        public static int FloorDouble(double p)
        {
            var i = (int)p;
            return i;
        }

        public static int FloorFloat(float p)
        {
            var i = (int) p;
            return p < 0 ? i - 1 : i;
        }
    }
}
