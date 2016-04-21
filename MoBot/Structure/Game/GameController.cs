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
        public Model model { get; private set; }       
        public GameController(Model model)
        {
            this.model = model;
            this.aiHandler = new AI.AIHandler(this);
        }   
        public Dictionary<int, Entity> entityList { get; private set; } = new Dictionary<int, Entity>();
        public AI.AIHandler aiHandler { get; private set; }
        public Player player { get; private set; }
        public World world { get; private set; } = new World();
        public DateTime lastMove = DateTime.Now;

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
            model.SendPacket(new PacketChat { message = message });                       
        }
        public LivingEntity createLivingEntity(int entityID, byte type)
        {
            LivingEntity entity = new LivingEntity();
            entityList.Add(entityID, entity);
            return entity;
        }
        private async Task updateMotion(bool OnGround = true)
        {
            await Task.Run(() =>
                model.SendPacket(new PacketPlayerPosLook
                {
                    X = player.x,
                    Y = player.y,
                    Z = player.z,
                    yaw = player.yaw,
                    pitch = player.pitch,
                    onGround = OnGround
                })
            );
        }
        public async Task respawn()
        {
            await Task.Run(() => model.SendPacket(new PacketClientStatus { Action = 0 }));
        }
        public async Task openInventory()
        {
            await Task.Run(() => model.SendPacket(new PacketClientStatus { Action = 2 }));
        }
        public async Task SetPlayerPos(double x, double y, double z)
        {
            bool onGround = true;
            bool moved = false;
            if (player.x != x || player.z != z)
                moved = true;
            player.x = x;
            if (y != player.y)
            {
                onGround = false; moved = true;
                player.y = y;
            }
            player.z = z;
            await updateMotion(onGround);
            if (moved)
                lastMove = DateTime.Now;
        }
    }
}
