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
        private Model model;
        public GameController(Model model)
        {
            this.model = model;
        }

        public Dictionary<int, Entity> entityList { get; private set; } = new Dictionary<int, Entity>();
        public Player player { get; private set; }
        public World world { get; private set; } = new World();
        public void CreatePlayer(int UID, String name = "")
        {
            player = new Player();
            entityList.Add(UID, player);
        }
        public Mob createMob(int ID, byte Type = 0)
        {
            Mob mob = new Mob() { Type = Type };
            entityList.Add(ID, mob);
            return mob; 
        }

        public void SendChatMessage(String message)
        {
            model.SendPacket(new PacketChat { message = message });
        }
    }
}
