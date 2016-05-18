using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MoBot.Structure.Game.World;

namespace MoBot.Structure.Game
{
    internal class GameController
    {
        private static GameController _instance;

        public static GameController GetInstance()
        {
            if (_instance == null)
                return _instance = new GameController();
            return _instance;
        }

        private GameController()
        {           
            ActionManager = new ActionManager(this);
        }

        private readonly ConcurrentDictionary<int, Entity> _entities = new ConcurrentDictionary<int, Entity>();   
        public Player Player { get; private set; }
        public GameWorld World { get; private set; } = new GameWorld();

        public ActionManager ActionManager { get; }

        public Entity GetEntity(int id)
        {
            Entity res;
            _entities.TryGetValue(id, out res);
            return res;
        }

        public Entity GetEntity()
        {
            return _entities.Values.FirstOrDefault();
        }

        public Entity GetEntity<T>() where T : Entity
        {
            return _entities.Values.OfType<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetEntities<T>()
        {
            return _entities.Values.OfType<T>();
        }

        public void RemoveEntity(int id)
        {
            Entity entity;
            _entities.TryRemove(id, out entity);
        }

        public void CreateUser(int uid, string name = "")
        {
            Player = CreatePlayer(uid, name);
        }

        public Player CreatePlayer(int uid, string name)
        {
            Player player = new Player(name);
            if (_entities.TryAdd(uid, player)) return player;
            Console.WriteLine($"Cannot add Entity {uid} to collection!");
            return null;
        }

        public Mob CreateMob(int entityId, byte type = 0)
        {
            Mob entity = new Mob {Type = type};
            if (_entities.TryAdd(entityId, entity)) return entity;
            Console.WriteLine($"Cannot add Entity {entityId} to collection!");
            return null;
        }

        public LivingEntity CreateLivingEntity(int entityId, byte type)
        {
            LivingEntity entity = new LivingEntity(entityId);
            if (_entities.TryAdd(entityId, entity)) return entity;
            Console.WriteLine($"Cannot add Entity {entityId} to collection!");
            return null;
        }

        public void Clear()
        {
            _entities.Clear();
            World = new GameWorld();
            Player = null;
        }

    }
}
