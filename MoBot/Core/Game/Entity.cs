using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using AForge.Math;
using Newtonsoft.Json;
using NLog;

namespace MoBot.Core.Game
{
    public class Entity : INotifyPropertyChanged
    {
        protected static Dictionary<int, string> EntityNames = new Dictionary<int, string>();
        protected static Logger Logger = LogManager.GetCurrentClassLogger();

        public static void LoadEntities()
        {
            try
            {
                var jsonFile = File.ReadAllText(Settings.EntitiesPath);
                dynamic entities = JsonConvert.DeserializeObject(jsonFile);
                foreach (var entityInfo in entities)
                {
                    EntityNames.Add((int)entityInfo.id, (string)entityInfo.name);
                }
            }
            catch (FileNotFoundException exception)
            {
                Logger.Warn($"Cant find {exception.FileName} file!");
            }
        }

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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
