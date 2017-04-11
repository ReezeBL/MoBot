using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using fNbt;
using MoBot.Structure.Game.AI;
using MoBot.Structure.Game.AI.Pathfinding;
using MoBot.Structure.Game.World;

namespace MoBot.Structure.Game
{
    public class GameController : INotifyPropertyChanged
    {
        private static GameController instance;
        public static GameController Instance => instance ?? (instance = new GameController());
        private GameController()
        {
            
        }
        private readonly ConcurrentDictionary<int, Entity> entities = new ConcurrentDictionary<int, Entity>();
        private readonly ConcurrentDictionary<Location, TileEntity> tileEntities = new ConcurrentDictionary<Location, TileEntity>();
        private Player player = new Player(0, "");

        public static Player Player
        {
            get => Instance.player;
            private set { Instance.player = value; Instance.OnPropertyChanged(nameof(Player)); }
        }

        public static GameWorld World { get; } = new GameWorld();      
        public static AiHandler AiHandler { get; } = new AiHandler();

        public static IList<Entity> LivingEntities { get; } = new BindingList<Entity>();
        public static IList<TileEntity> TileEntities { get; } = new BindingList<TileEntity>();

        public static Entity GetEntity(int id)
        {
            Entity res;
            Instance.entities.TryGetValue(id, out res);
            return res;
        }

        public static TileEntity SetTileEntity(Location location, NbtCompound tag, Dictionary<string, object> tags = null)
        {
            TileEntity entity;
            if (Instance.tileEntities.ContainsKey(location))
            {
                Instance.tileEntities.TryGetValue(location, out entity);
                if (entity != null)
                {
                    entity.Root = tag;
                    entity.Tags = tags;
                }
            }
            else
            {
                entity = new TileEntity
                {
                    Location = location,
                    Root = tag,
                    Id = World.GetBlock(location),
                    Tags = tags
                };
                Instance.tileEntities.TryAdd(location, entity);
                if (entity.Id == 432)
                    TileEntities.Add(entity);
            }
            return entity;
        }

        public static Entity GetEntity()
        {
            return Instance.entities.Values.FirstOrDefault();
        }

        public static Entity GetEntity<T>() where T : Entity
        {
            return Instance.entities.Values.OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetEntities<T>()
        {
            return Instance.entities.Values.OfType<T>();
        }

        public static void RemoveEntity(int id)
        {
            Entity entity;
            Instance.entities.TryRemove(id, out entity);
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
            if (Instance.entities.TryAdd(uid, player)) return player;
            Console.WriteLine($"Cannot add Entity {uid} to collection!");
            return null;
        }

        public static Mob CreateMob(int entityId, byte type = 0)
        {
            var entity = new Mob(entityId) {Type = type};
            LivingEntities.Add(entity);
            if (Instance.entities.TryAdd(entityId, entity)) return entity;
            Console.WriteLine($"Cannot add Entity {entityId} to collection!");
            return null;
        }

        public static LivingEntity CreateLivingEntity(int entityId, byte type)
        {
            var entity = new LivingEntity(entityId);
            if (Instance.entities.TryAdd(entityId, entity)) return entity;
            Console.WriteLine($"Cannot add Entity {entityId} to collection!");
            return null;
        }

        public static Entity CreateEntity(int entityId, byte type)
        {
            var entity = new Entity(entityId);
            if (Instance.entities.TryAdd(entityId, entity)) return entity;
            Console.WriteLine($"Cannot add Entity {entityId} to collection!");
            return null;
        }

        public static void Clear()
        {
            Instance.entities.Clear();
            Instance.tileEntities.Clear();
            TileEntities.Clear();
            LivingEntities.Clear();
            World.Clear();
            Player = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
