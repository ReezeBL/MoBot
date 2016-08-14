using System.ComponentModel;
using System.Runtime.CompilerServices;
using AForge.Math;
using MoBot.Annotations;

namespace MoBot.Structure.Game
{
    public class Entity : INotifyPropertyChanged
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
            OnPropertyChanged(nameof(Position));
        }

        public void Move(double dx, double dy, double dz)
        {
            Position += new Vector3((float) dx, (float) dy, (float) dz);
            OnPropertyChanged(nameof(Position));
        }

        public void SetPosition(Vector3 newPos)
        {
            Position = newPos;
            OnPropertyChanged(nameof(Position));
        }

        public void Move(Vector3 dir)
        {
            Position += dir;
            OnPropertyChanged(nameof(Position));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
