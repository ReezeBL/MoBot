﻿using MoBot.Structure.Game.AI;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private readonly Dictionary<int, Entity> _entityList = new Dictionary<int, Entity>();        
        public Player Player { get; private set; }
        public World World { get; private set; } = new World();

        public ActionManager ActionManager { get; }

        public Entity GetEntity(int id)
        {
            Entity res;
            _entityList.TryGetValue(id, out res);
            return res;
        }

        public Entity GetEntity()
        {
            return _entityList.Values.FirstOrDefault();
        }

        public Entity GetEntity<T>() where T : Entity
        {
            return _entityList.Values.OfType<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetEntities<T>()
        {
            return _entityList.Values.OfType<T>();
        }

        public void RemoveEntity(int id)
        {
            if (_entityList.ContainsKey((id)))
                _entityList.Remove(id);
        }

        public void CreateUser(int uid, string name = "")
        {
            Player = CreatePlayer(uid, name);
        }

        public Player CreatePlayer(int uid, string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Player player = new Player(name);
            _entityList.Add(uid, player);
            return player;
        }

        public Mob CreateMob(int id, byte type = 0)
        {
            Mob mob = new Mob {Type = type};
            _entityList.Add(id, mob);
            return mob;
        }

        public LivingEntity CreateLivingEntity(int entityId, byte type)
        {
            LivingEntity entity = new LivingEntity();
            _entityList.Add(entityId, entity);
            return entity;
        }
    }
}
