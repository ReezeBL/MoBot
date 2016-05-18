using System;
using System.Threading;
using MoBot.Protocol.Packets.Play;
using MoBot.Structure.Game.AI.Pathfinding;

namespace MoBot.Structure.Game
{
    internal class ActionManager
    {
        private static int _transactionId = 1;
        private readonly GameController _gameController;
        public DateTime LastMove = DateTime.Now;

        public ActionManager(GameController gameController)
        {
            _gameController = gameController;
        }

        public void SendChatMessage(string message)
        {
            Model.GetInstance().SendPacket(new PacketChat { message = message });                       
        }

        public void UpdateMotion()
        {

            Model.GetInstance().SendPacket(new PacketPlayerPosLook
            {
                X = _gameController.Player.X,
                Y = _gameController.Player.Y,
                Z = _gameController.Player.Z,
                yaw = _gameController.Player.Yaw,
                pitch = _gameController.Player.Pitch,
                onGround = _gameController.Player.OnGround
            });

        }

        public void Respawn()
        {
            Model.GetInstance().SendPacket(new PacketClientStatus {Action = 0});
        }

        public void OpenInventory()
        {
            Model.GetInstance().SendPacket(new PacketClientStatus {Action = 2});
        }

        public void SetPlayerPos(double x, double y, double z)
        {            
            double dx = x - _gameController.Player.X;
            double dy = y - _gameController.Player.Y;
            double dz = z - _gameController.Player.Z;
            bool moved = dx * dx + dy * dy + dz * dz >= 9e-4;
            _gameController.Player.OnGround = Math.Abs(dy) >= 0.1;
            _gameController.Player.X = x;
            _gameController.Player.Y = y;
            _gameController.Player.Z = z;
            UpdateMotion();
            if (moved)
                LastMove = DateTime.Now;
        }

        public void RotatePlayer(double x, double y, double z)
        {
            double r = Math.Sqrt(x * x + y * y + z * z);
            double yaw = -Math.Atan2(x, z) / Math.PI * 180;
            double pitch = -Math.Asin(y / r) / Math.PI * 180;
            _gameController.Player.Yaw = (float)yaw;
            _gameController.Player.Pitch = (float)pitch;
        }

        public void ClickInventorySlot(int slot)
        {
            Model.GetInstance()
                .SendPacket(new PacketClickWindow
                {
                    WindowID = 0,
                    Mode = 0,
                    ActionNumber = (short) _transactionId++,
                    Button = 0,
                    Slot = (short) slot,
                    ItemStack = _gameController.Player.Inventory[slot]
                });
        }

        public void ExchangeInventorySlots(int slot1, int slot2)
        {
            ClickInventorySlot(slot1);
            Thread.Sleep(100);
            ClickInventorySlot(slot2);
            Thread.Sleep(100);
            ClickInventorySlot(slot1);
        }

        public void MoveToLocation(PathPoint endPoint)
        {
            PathFinder pf = new PathFinder();
            var path = pf.DynamicPath(_gameController.Player, endPoint);
            for (var point = path.Current; point != null; path.MoveNext(), point = path.Current)
            {
                
            } 
        }
    }
}