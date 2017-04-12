using System.Collections.Generic;
using MoBot.Core.Game.Items;

namespace MoBot.Core.Game
{
    public class Player : LivingEntity
    {
        public Container Inventory => containers[0];

        public Container CurrentContainer
        {
            get => currentContainer;
            private set
            {
                currentContainer = value;
                OnPropertyChanged(nameof(CurrentContainer));
            }
        }

        public int HeldItemBar = 0;
        public int HeldItem => HeldItemBar + 36;
        public bool OnGround;

        private short food = 20;
        private float saturation;
        public string Name;

        public Item GetHeldItem => Inventory[HeldItem]?.Item;
        public ItemStack GetHeldItemStack => Inventory[HeldItem];

        public short Food
        {
            get => food;

            set
            {
                food = value;
                OnPropertyChanged(nameof(Food));
            }
        }

        public float Saturation
        {
            get => saturation;

            set
            {
                saturation = value;
                OnPropertyChanged(nameof(Saturation));
            }
        }

        public float GetDigSpeed(Block block)
        {
            var heldItemStack = Inventory[HeldItem];
            var heldItem = heldItemStack.Item;

            var strength = heldItem.GetItemStrength(heldItemStack, block)/block.Hardness/
                           (heldItem.CanHarvest(block) ? 30 : 100);

            if (!OnGround) strength /= 5;
            if (InWater()) strength /= 5;

            var playerHead = Position;
            playerHead.Y += 1.62f;


            return strength;
        }

        public bool InWater()
        {
            var playerHead = Position;
            playerHead.Y += 1.62f;

            return Block.Water.Contains(GameController.World.GetBlock(playerHead));
        }

        private readonly Dictionary<int, Container> containers = new Dictionary<int, Container>
        {
            {0, new Container(9)},
            {255, new Container(1)}
        };

        private Container currentContainer;

        public Container CreateContainer(int windowId, int capacity)
        {
            containers.Add(windowId, new Container(capacity, (byte) windowId));
            CurrentContainer = containers[windowId];
            return CurrentContainer;
        }

        public void CloseContainer(int windowId)
        {
            if (windowId == 0)
                return;

            containers.Remove(windowId);
            CurrentContainer = containers[0];
        }

        public Container GetContainer(int windowId)
        {
            return containers.ContainsKey(windowId) ? containers[windowId] : null;
        }

        public Player(int id, string name) : base(id)
        {
            Name = name;
            CurrentContainer = containers[0];
        }

        public override string ToString()
        {
            return $"Player: {Name}, ({MathHelper.FloorFloat(X)} | {(int) Y} | {MathHelper.FloorFloat(Z)})";
        }
    }
}
