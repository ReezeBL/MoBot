﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MoBot.Structure.Game.AI.Pathfinding;
using MoBot.Structure.Game.Items;

namespace MoBot.Structure.Game
{
    public class Player : LivingEntity
    {

        public Container Inventory => _containers[0];
        public Container CurrentContainer { get; private set; }

        public int HeldItemBar = 0;
        public int HeldItem => HeldItemBar + 36;
        public bool OnGround;

        public short Food = 20;
        public float Saturation;
        public string Name;

        public Item GetHeldItem => Inventory[HeldItem].Item;
        public ItemStack GetHeldItemStack => Inventory[HeldItem];

        public float GetDigSpeed(Block block)
        {
            ItemStack heldItemStack = Inventory[HeldItem];
            Item heldItem = heldItemStack.Item;

            float strength = heldItem.GetItemStrength(heldItemStack, block) / block.Hardness / (heldItem.CanHarvest(block) ? 30 : 100);

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

        private readonly Dictionary<int, Container> _containers = new Dictionary<int, Container> {{0, new Container(9)}, {255, new Container(1)} };

        public Container CreateContainer(int windowId, int capacity)
        {
            _containers.Add(windowId, new Container(capacity, Inventory, (byte)windowId));
            CurrentContainer = _containers[windowId];
            return CurrentContainer;
        }

        public void CloseContainer(int windowId)
        {
            if (windowId == 0)
                return;

            _containers.Remove(windowId);
            CurrentContainer = _containers[0];
        }

        public Container GetContainer(int windowId)
        {
            return _containers.ContainsKey(windowId) ? _containers[windowId] : null;
        }

        public Player(int id, string name) : base(id)
        {
            Name = name;
            CurrentContainer = _containers[0];
        }

        public override string ToString()
        {
            return $"Player: {Name}, ({MathHelper.floor_float(X)} | {(int)Y} | {MathHelper.floor_float(Z)})";
        }
    }
}
