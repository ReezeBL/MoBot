using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MoBot.Annotations;
using MoBot.Structure.Game.AI;
using MoBot.Structure.Game.World;

namespace MoBot.Structure.Game
{
    public class GameController : INotifyPropertyChanged
    {
        private static GameController _instance;
        public static GameController Instance => _instance ?? (_instance = new GameController());
        private GameController()
        {
            
        }
        private readonly ConcurrentDictionary<int, Entity> _entities = new ConcurrentDictionary<int, Entity>();
        private Player _player = new Player(0, "");

        public static Player Player
        {
            get { return Instance._player; }
            private set { Instance._player = value; Instance.OnPropertyChanged(nameof(Player)); }
        }

        public static GameWorld World { get; } = new GameWorld();      
        public static AiHandler AiHandler { get; private set; } = new AiHandler();

        public static IList<Entity> LivingEntities { get; } = new BindingList<Entity>();

        public static Entity GetEntity(int id)
        {
            Entity res;
            Instance._entities.TryGetValue(id, out res);
            return res;
        }

        public static Entity GetEntity()
        {
            return Instance._entities.Values.FirstOrDefault();
        }

        public static Entity GetEntity<T>() where T : Entity
        {
            return Instance._entities.Values.OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetEntities<T>()
        {
            return Instance._entities.Values.OfType<T>();
        }

        public static void RemoveEntity(int id)
        {
            Entity entity;
            Instance._entities.TryRemove(id, out entity);
            LivingEntities.Remove(entity);
        }

        public static void CreateUser(int uid, string name = "")
        {
            Player = CreatePlayer(uid, name);
        }

        public static Player CreatePlayer(int uid, string name)
        {
            var player = new Player(uid, name);
            LivingEntities.Add(player);
            if (Instance._entities.TryAdd(uid, player)) return player;
            Console.WriteLine($"Cannot add Entity {uid} to collection!");
            return null;
        }

        public static Mob CreateMob(int entityId, byte type = 0)
        {
            Mob entity = new Mob(entityId) {Type = type};
            LivingEntities.Add(entity);
            if (Instance._entities.TryAdd(entityId, entity)) return entity;
            Console.WriteLine($"Cannot add Entity {entityId} to collection!");
            return null;
        }

        public static LivingEntity CreateLivingEntity(int entityId, byte type)
        {
            LivingEntity entity = new LivingEntity(entityId);
            if (Instance._entities.TryAdd(entityId, entity)) return entity;
            Console.WriteLine($"Cannot add Entity {entityId} to collection!");
            return null;
        }

        public static Entity CreateEntity(int entityId, byte type)
        {
            Entity entity = new Entity(entityId);
            if (Instance._entities.TryAdd(entityId, entity)) return entity;
            Console.WriteLine($"Cannot add Entity {entityId} to collection!");
            return null;
        }

        public static void Clear()
        {
            Instance._entities.Clear();
            World.Clear();
            Player = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
