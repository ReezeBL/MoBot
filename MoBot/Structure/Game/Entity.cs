namespace MoBot.Structure.Game
{
    internal class Entity
    {
        public double X, Y, Z;
        public int Id;
        public Entity(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Entity(int id)
        {
            Id = id;
        }
    }
}
