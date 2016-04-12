namespace MoBot.Structure.Game
{
    class Mob : LivingEntity
    {
        public byte Type;
        public override string ToString()
        {
            return $"Mob : {Type} ({(int)x} | {(int)y} | {(int)z})";
        }
    }
}