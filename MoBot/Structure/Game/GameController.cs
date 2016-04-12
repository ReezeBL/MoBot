using MinecraftEmuPTS.GameData;
using MoBot.Protocol.Packets.Play;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure.Game
{
    class GameController
    {
        private NLog.Logger log = Program.getLogger();
        private Model model;       
        public GameController(Model model)
        {
            this.model = model;
        }

        public Dictionary<int, Entity> entityList { get; private set; } = new Dictionary<int, Entity>();
        public Player player { get; private set; }
        public World world { get; private set; } = new World();
        public void CreateUser(int UID, String name = "")
        {
            player = CreatePlayer(UID, name);
            
        }
        public Player CreatePlayer(int UID, String name)
        {
            Player player = new Player(name);
            entityList.Add(UID, player);
            return player;
        }
        public Mob createMob(int ID, byte Type = 0)
        {
            Mob mob = new Mob() { Type = Type };
            entityList.Add(ID, mob);
            return mob; 
        }

        public void SendChatMessage(String message)
        {
            if(!message.StartsWith("-"))
                model.SendPacket(new PacketChat { message = message });
            else
            {
                if(message == "-elist")
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(Entity e in model.controller.entityList.Values)
                    {
                        sb.AppendLine(e.ToString());
                    }
                    model.viewer.OnNext(new Actions.ActionMessage { message = sb.ToString() });
                }
            }
        }

        internal LivingEntity createLivingEntity(int entityID, byte type)
        {
            LivingEntity entity = new LivingEntity();
            entityList.Add(entityID, entity);
            return entity;
        }
    }
}
