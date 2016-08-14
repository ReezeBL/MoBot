using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MoBot.Structure.Game.AI;
using MoBot.Structure.Game.World;

namespace MoBot.Structure.Game
{
    public static class GameController
    {
        private static readonly ConcurrentDictionary<int, Entity> _entities = new ConcurrentDictionary<int, Entity>();
        public static Player Player { get; private set; }
        public static GameWorld World { get; } = new GameWorld();      
        public static AiHandler AiHandler { get; private set; } = new AiHandler();

        public static List<Entity> Entities => _entities.Values.ToList();

        public static Entity GetEntity(int id)
        {
            Entity res;
            _entities.TryGetValue(id, out res);
            return res;
        }

        public static Entity GetEntity()
        {
            return _entities.Values.FirstOrDefault();
        }

        public static Entity GetEntity<T>() where T : Entity
        {
            return _entities.Values.OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetEntities<T>()
        {
            return _entities.Values.OfType<T>();
        }

        public static void RemoveEntity(int id)
        {
            Entity entity;
            _entities.TryRemove(id, out entity);
        }

        public static void CreateUser(int uid, string name = "")
        {
            Player = CreatePlayer(uid, name);
        }

        public static Player CreatePlayer(int uid, string name)
        {
            var player = new Player(uid, name);
            if (_entities.TryAdd(uid, player)) return player;
            Console.WriteLine($"Cannot add Entity {uid} to collection!");
            return null;
        }

        public static Mob CreateMob(int entityId, byte type = 0)
        {
            Mob entity = new Mob(entityId) {Type = type};
            if (_entities.TryAdd(entityId, entity)) return entity;
            Console.WriteLine($"Cannot add Entity {entityId} to collection!");
            return null;
        }

        public static LivingEntity CreateLivingEntity(int entityId, byte type)
        {
            LivingEntity entity = new LivingEntity(entityId);
            if (_entities.TryAdd(entityId, entity)) return entity;
            Console.WriteLine($"Cannot add Entity {entityId} to collection!");
            return null;
        }

        public static Entity CreateEntity(int entityId, byte type)
        {
            Entity entity = new Entity(entityId);
            if (_entities.TryAdd(entityId, entity)) return entity;
            Console.WriteLine($"Cannot add Entity {entityId} to collection!");
            return null;
        }

        public static void Clear()
        {
            _entities.Clear();
            World.Clear();
            Player = null;
        }

    }
}
