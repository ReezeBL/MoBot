using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MoBot.Structure.Game.AI;
using MoBot.Structure.Game.World;

namespace MoBot.Structure.Game
{
    public class GameController
    {
        private static GameController _instance;

        private static GameController GetInstance()
        {
            if (_instance == null)
                return _instance = new GameController();
            return _instance;
        }

        private GameController()
        {           
            
        }

        private readonly ConcurrentDictionary<int, Entity> _entities = new ConcurrentDictionary<int, Entity>();
        private Player _player;
        private readonly GameWorld _world = new GameWorld();      
        public static GameWorld World => _instance?._world;       
        public static AiHandler AiHandler { get; private set; } = new AiHandler(new BasicRoutine());
        public static Player Player => _instance?._player;

        public static Entity GetEntity(int id)
        {
            Entity res;
            GetInstance()._entities.TryGetValue(id, out res);
            return res;
        }

        public static Entity GetEntity()
        {
            return GetInstance()._entities.Values.FirstOrDefault();
        }

        public static Entity GetEntity<T>() where T : Entity
        {
            return GetInstance()._entities.Values.OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetEntities<T>()
        {
            return GetInstance()._entities.Values.OfType<T>();
        }

        public static void RemoveEntity(int id)
        {
            Entity entity;
            GetInstance()._entities.TryRemove(id, out entity);
        }

        public static void CreateUser(int uid, string name = "")
        {
            GetInstance()._player = CreatePlayer(uid, name);
        }

        public static Player CreatePlayer(int uid, string name)
        {
            var player = new Player(uid, name);
            if (GetInstance()._entities.TryAdd(uid, player)) return player;
            Console.WriteLine($"Cannot add Entity {uid} to collection!");
            return null;
        }

        public static Mob CreateMob(int entityId, byte type = 0)
        {
            Mob entity = new Mob(entityId) {Type = type};
            if (GetInstance()._entities.TryAdd(entityId, entity)) return entity;
            Console.WriteLine($"Cannot add Entity {entityId} to collection!");
            return null;
        }

        public static LivingEntity CreateLivingEntity(int entityId, byte type)
        {
            LivingEntity entity = new LivingEntity(entityId);
            if (GetInstance()._entities.TryAdd(entityId, entity)) return entity;
            Console.WriteLine($"Cannot add Entity {entityId} to collection!");
            return null;
        }

        public static void Clear()
        {
            var instance = GetInstance();
            instance._entities.Clear();
            instance._world.Clear();
            _instance._player = null;
        }

    }
}
