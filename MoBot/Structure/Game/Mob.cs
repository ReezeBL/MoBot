namespace MoBot.Structure.Game
{
    internal class Mob : LivingEntity
    {
        public byte Type;
        public override string ToString()
        {
            return $"Mob : {Type} ({(int)X} | {(int)Y} | {(int)Z})";
        }

        public Mob(double x, double y, double z) : base(x, y, z)
        {
            Type = 0;
        }

        public Mob()
        {
        }
    }
}