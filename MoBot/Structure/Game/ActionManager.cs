using System;
using System.Threading;
using AForge.Math;
using MoBot.Protocol.Packets.Play;
using MoBot.Structure.Game.AI.Pathfinding;

namespace MoBot.Structure.Game
{
    public class ActionManager
    {
        private static int _transactionId = 1;       
        public static DateTime LastMove = DateTime.Now;

        private ActionManager()
        {
            
        }

        public static void SendChatMessage(string message)
        {
            NetworkController.SendPacket(new PacketChat { Message = message });                       
        }

        public static void UpdatePosition()
        {
            var player = GameController.Player;
            NetworkController.SendPacket(new PacketPlayerPosLook
            {              
                X = player.X,
                Y = player.Y,
                Z = player.Z,
                Yaw = player.Yaw,
                Pitch = player.Pitch,
                OnGround = player.OnGround
            });

        }

        public static void Respawn()
        {
            NetworkController.SendPacket(new PacketClientStatus {Action = 0});
        }       

        public static void SetPlayerPos(double x, double y, double z)
        {
            var player = GameController.Player;
            var dx = x - player.X;
            var dy = y - player.Y;
            var dz = z - player.Z;
            var moved = dx*dx + dy*dy + dz*dz >= 9e-4;
            player.OnGround = Math.Abs(dy) >= 0.1;
            player.SetPosition(x,y,z);
            UpdatePosition();
            if (moved)
                LastMove = DateTime.Now;
        }

        public static void SetPlayerPos(Vector3 newPos)
        {
            var player = GameController.Player;
            var dir = player.Position - newPos;
            var moved = dir.Square >= 9e-4;
            player.OnGround = Math.Abs(dir.Y) >= 0.1;
            player.SetPosition(newPos);
            UpdatePosition();
            if (moved)
                LastMove = DateTime.Now;
        }

        public static void MovePlayer(double dx, double dy, double dz)
        {
            var player = GameController.Player;
            var moved = dx * dx + dy * dy + dz * dz >= 9e-4;
            player.OnGround = Math.Abs(dy) >= 0.1;
            player.Move(dx, dy, dz);
            UpdatePosition();
            if (moved)
                LastMove = DateTime.Now;
        }

        public static void MovePlayer(Vector3 dir)
        {
            var player = GameController.Player;
            var moved = dir.Square >= 9e-4;
            player.OnGround = Math.Abs(dir.Y) >= 0.1;
            player.Move(dir);
            UpdatePosition();
            if (moved)
                LastMove = DateTime.Now;
        }

        public static void RotatePlayer(double x, double y, double z)
        {
            var player = GameController.Player;
            double r = Math.Sqrt(x * x + y * y + z * z);
            double yaw = -Math.Atan2(x, z) / Math.PI * 180;
            double pitch = -Math.Asin(y / r) / Math.PI * 180;
            player.Yaw = (float)yaw;
            player.Pitch = (float)pitch;
        }

        public static void ClickInventorySlot(int slot)
        {
            NetworkController.SendPacket(new PacketClickWindow
                {
                    WindowId = 0,
                    Mode = 0,
                    ActionNumber = (short) _transactionId++,
                    Button = 0,
                    Slot = (short) slot,
                    ItemStack = GameController.Player.Inventory[slot]
                });
        }

        public static void ExchangeInventorySlots(int slot1, int slot2)
        {
            ClickInventorySlot(slot1);
            Thread.Sleep(100);
            ClickInventorySlot(slot2);
            Thread.Sleep(100);
            ClickInventorySlot(slot1);
        }

        public static void MoveToLocation(PathPoint endPoint)
        {
            var path = PathFinder.DynamicPath(GameController.Player, endPoint);
            foreach(var point in path)             
                SmoothMove(point);

        }
        public static void MoveToLocationS(PathPoint endPoint)
        {
            
            var path = PathFinder.StaticPath(GameController.Player, endPoint);
            foreach (var point in path)
                SmoothMove(point);
        }

        private static void SmoothMove(PathPoint point)
        {
            var speed = 0.6f;          
            var player = GameController.Player;           
            while (true)
            {              
                var vertical = new Vector3(0f, point.Y - player.Y, 0f);               
                var horizontal = new Vector3(point.X + 0.5f * Math.Sign(point.X) - player.X, 0, point.Z + 0.5f * Math.Sign(point.Z) - player.Z);               
                var dir = vertical + horizontal;
                if(dir.Norm < 0.1)
                    break;
                if (horizontal.Norm < speed)
                    speed = horizontal.Norm;                                                                                                   
                RotatePlayer(dir.X, dir.Y, dir.Z);
                if (vertical.Y > 0.1f)
                    MovePlayer(vertical);
                else if (horizontal.Norm > 0.1f)
                {
                    horizontal.Normalize();
                    MovePlayer(horizontal*speed);
                }
                else
                    MovePlayer(vertical);
                Thread.Sleep(50);
            }
        }
    }
}