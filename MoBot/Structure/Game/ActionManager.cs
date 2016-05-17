using System;
using System.Threading;
using System.Threading.Tasks;
using MoBot.Protocol.Packets.Play;

namespace MoBot.Structure.Game
{
    class ActionManager
    {
        private static int _transactionId = 1;
        private GameController _gameController;
        public DateTime LastMove = DateTime.Now;

        public ActionManager(GameController gameController)
        {
            _gameController = gameController;
        }

        public void SendChatMessage(String message)
        {
            Model.GetInstance().SendPacket(new PacketChat { message = message });                       
        }

        public async Task UpdateMotion()
        {
            await Task.Run(() =>
                Model.GetInstance().SendPacket(new PacketPlayerPosLook
                {
                    X = _gameController.Player.X,
                    Y = _gameController.Player.Y,
                    Z = _gameController.Player.Z,
                    yaw = _gameController.Player.Yaw,
                    pitch = _gameController.Player.Pitch,
                    onGround = _gameController.Player.OnGround
                })
                );
        }

        public async Task Respawn()
        {
            await Task.Run(() => Model.GetInstance().SendPacket(new PacketClientStatus { Action = 0 }));
        }

        public async Task OpenInventory()
        {
            await Task.Run(() => Model.GetInstance().SendPacket(new PacketClientStatus { Action = 2 }));
        }

        public async Task SetPlayerPos(double x, double y, double z)
        {            
            double dx = x - _gameController.Player.X;
            double dy = y - _gameController.Player.Y;
            double dz = z - _gameController.Player.Z;
            bool moved = dx * dx + dy * dy + dz * dz >= 9e-4;
            _gameController.Player.OnGround = Math.Abs(dy) >= 0.1;
            _gameController.Player.X = x;
            _gameController.Player.Y = y;
            _gameController.Player.Z = z;
            await UpdateMotion();
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
            Model.GetInstance().SendPacket(new PacketClickWindow { WindowID = 0, Mode = 0, ActionNumber = (short) _transactionId++, Button = 0, Slot = (short)slot, ItemStack = _gameController.Player.Inventory[slot] });
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