﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MoBot.Structure.Game.AI.Pathfinding;
using MoBot.Structure.Game.Items;
using MoBot.Structure.Game.World;
using NLog;
using TreeSharp;
using Action = TreeSharp.Action;

namespace MoBot.Structure.Game.AI.Tasks
{
    public class CustomEvents : Task
    {
        public int MinFoodDanger = 5;
        public double MinHealthDanger = 5;

        public string TeleportHome;
        public string TeleportCheckpoint;
        public string CreateCheckpoint;
        public string RemoveCheckpoint;

        public Location Chest = new Location(-4533, 65, -1318);
        public bool Store;

        private IEnumerator _routine;
        private readonly Logger _logger = Program.GetLogger();

        private bool IsHungry(object context)
        {
            if (GameController.Player.Food > MinFoodDanger) return false;

            _routine = StartRoutine(Feed());
            return true;
        }

        private bool InventoryIsFool(object context)
        {
            if (GameController.Player.CurrentContainer.InventoryFreeSlot != -1 && !Store) return false;

            Store = false;
            _routine = StartRoutine(HomeRun());
            return true;
        }

        private IEnumerator Feed()
        {
            _logger.Info("I'm hungry!");
            yield return SwitchToFood();
            if (GameController.Player.GetHeldItem is ItemFood)
            {
                Console.WriteLine($"Eating {GameController.Player.GetHeldItem.Name}");
                ActionManager.UseItem();
                for (int i = 0; i < 32; i++)
                {
                    yield return _awaiter;
                    ActionManager.UpdatePosition();
                }
            }
            yield return _awaiter;
        }

        private IEnumerator SwitchToFood()
        {
            var heldItem = GameController.Player.GetHeldItem;

            if (heldItem is ItemFood)
            {
                yield break;
            }
            var belt = GameController.Player.Inventory.Belt;
            var item = belt.FirstOrDefault(slot => slot.Item is ItemFood);

            if (item != null)
            {
                Console.WriteLine($"Selecting {item.Item.Name} in the belt at {item.Slot}");
                ActionManager.SelectBeltSlot(item.Slot);

                yield return _awaiter;
                yield break;
            }

            var items =
               GameController.Player.Inventory.IndexedInventory;
            item = items.FirstOrDefault(slot => slot.Item is ItemFood);

            if (item == null)
            {
                MinFoodDanger = -1;
                _logger.Info("No food in inventory! Disabling autofeeding");
                yield break;
            }

            Console.WriteLine($"Selecting {item.Item.Name} at {item.Slot}");
            ActionManager.SelectBeltSlot(item.Slot);

            yield return _awaiter;
        }

        private RunStatus RunRoutine(object context)
        {
            if(_routine == null || !_routine.MoveNext())
                return RunStatus.Success;
            return RunStatus.Running;
        }

        private IEnumerator HomeRun()
        {
            _logger.Info("Perform homerun");
            ActionManager.SendChatMessage(RemoveCheckpoint);

            yield return WaitForSeconds(4000);
            ActionManager.SendChatMessage(CreateCheckpoint);

            yield return WaitForSeconds(4000);
            ActionManager.SendChatMessage(TeleportHome);

            yield return WaitForSeconds(1000);
            ActionManager.RightClick(Chest.X, Chest.Y, Chest.Z);

            yield return WaitForSeconds(1500);

            foreach (var slot in GameController.Player.CurrentContainer.IndexedInventory)
            {
                if (Settings.KeepItems.Contains(slot.Item.Id) || slot.Item.Id == -1) continue;
                int freeSlot = GameController.Player.CurrentContainer.ContainerFreeSlot;
                if (freeSlot == -1) break;
                yield return ActionManager.ExchangeInventorySlots(slot.Slot, freeSlot);

                yield return null;
                yield return null;
            }

            ActionManager.CloseWindow();

            yield return WaitForSeconds(500);
            ActionManager.SendChatMessage(TeleportCheckpoint);

            yield return WaitForSeconds(1500);
        }

        private bool IsDead(object context)
        {
            return Math.Abs(GameController.Player.Health) < 1;
        }

        private void Ressurect()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("You are dead!");
            sb.AppendLine("----Nearby entities: ");
            foreach (var entity in GameController.GetEntities<LivingEntity>())
            {
                sb.AppendLine(entity.ToString());
            }

            _logger.Info(sb.ToString);
            ActionManager.Respawn();
        }

        public CustomEvents()
        {
            _root = new PrioritySelector(new Decorator(IsDead, new Action(o => Ressurect())), new Decorator(IsHungry, new Action(RunRoutine)), new Decorator(InventoryIsFool, new Action(RunRoutine)));
        }

        public void GenerateStrings()
        {
            string name = GameController.Player.Name;
            TeleportCheckpoint = $"/warp blpoint{name}";
            TeleportHome = $"/warp {Settings.HomeWarp}";
            CreateCheckpoint = $"/warp pcreate blpoint{name}";
            RemoveCheckpoint = $"/warp remove blpoint{name}";
        }

        public override int GetPriority()
        {
            return 0;
        }
    }
}
