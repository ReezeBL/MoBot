using AForge.Math;

namespace MoBot.Structure.Game
{
    internal class Entity
    {
        public float X => Position.X;
        public float Y => Position.Y;
        public float Z => Position.Z;

        public Vector3 Position { get; protected set; }
        public int Id;       

        public Entity(int id)
        {
            Id = id;
        }

        public void SetPosition(double x, double y, double z)
        {
            Position = new Vector3((float) x, (float) y , (float) z);
        }

        public void Move(double dx, double dy, double dz)
        {
            Position += new Vector3((float) dx, (float) dy, (float) dz);
        }

        public void SetPosition(Vector3 newPos)
        {
            Position = newPos;
        }

        public void Move(Vector3 dir)
        {
            Position += dir;
        }
    }
}
