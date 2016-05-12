using MinecraftEmuPTS.GameData;
using MoBot.Protocol.Packets.Play;
using MoBot.Structure.Game.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoBot.Structure.Game
{
    class GameController
    {
        private NLog.Logger log = Program.getLogger();
        private static int TransactionID = 1;
        public Model model { get; private set; }       
        public GameController(Model model)
        {
            this.model = model;
            aiHandler = new AIHandler(this);
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
        public async Task updateMotion()
        {
            await Task.Run(() =>
                model.SendPacket(new PacketPlayerPosLook
                {
                    X = player.x,
                    Y = player.y,
                    Z = player.z,
                    yaw = player.yaw,
                    pitch = player.pitch,
                    onGround = player.onGround
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
            double dx = x - player.x;
            double dy = y - player.y;
            double dz = z - player.z;
            bool moved = dx * dx + dy * dy + dz * dz >= 9e-4;
            player.onGround = Math.Abs(dy) >= 0.1;
            player.x = x; player.y = y; player.z = z;
            await updateMotion();
            if (moved)
                lastMove = DateTime.Now;
        }
        public void RotatePlayer(double x, double y, double z)
        {
            double r = Math.Sqrt(x * x + y * y + z * z);
            double yaw = -Math.Atan2(x, z) / Math.PI * 180;
            double pitch = -Math.Asin(y / r) / Math.PI * 180;
            player.yaw = (float)yaw;
            player.pitch = (float)pitch;
        }
        public void ClickInventorySlot(int slot)
        {
            model.SendPacket(new PacketClickWindow { WindowID = 0, Mode = 0, ActionNumber = (short)TransactionID++, Button = 0, Slot = (short)slot, ItemStack = player.inventory[slot] });
        }
        public void ExchangeInventorySlots(int slot1, int slot2)
        {
            ClickInventorySlot(slot1);
            Thread.Sleep(100);
            ClickInventorySlot(slot2);
            Thread.Sleep(100);
            ClickInventorySlot(slot1);
        }
    }
}
