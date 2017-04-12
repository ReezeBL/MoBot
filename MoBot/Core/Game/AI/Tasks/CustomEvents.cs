using System;
using System.Collections;
using System.Linq;
using System.Text;
using MoBot.Core.Game.AI.Pathfinding;
using MoBot.Core.Game.Items;
using NLog;
using TreeSharp;
using Action = TreeSharp.Action;

namespace MoBot.Core.Game.AI.Tasks
{
    public class CustomEvents : Task
    {
        public static readonly Location Chest = new Location(-2857, 64, 2919);
        public static readonly Location StoreChest = new Location(-2858, 64, 2919);
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly object monitor = new object();

        private IEnumerator routine;
        public string CreateCheckpoint;
        public int MinFoodDanger = 16;
        public double MinHealthDanger = 12;
        public string RemoveCheckpoint;
        public bool Store;
        public string TeleportBack;
        public string TeleportCheckpoint;

        public string TeleportHome;

        public CustomEvents()
        {
            Root = new PrioritySelector(new Decorator(IsDead, new Action(RunRoutine)),
                new Decorator(IsInDanger, new Action(RunRoutine)),
                new Decorator(IsHungry, new Action(RunRoutine)), new Decorator(InventoryIsFool, new Action(RunRoutine)));
        }

        private IEnumerator Routine
        {
            get
            {
                lock (monitor)
                    return routine;
            }

            set
            {
                lock (monitor)
                    routine = value;
            }
        }

        public IEnumerator Feed()
        {
            Log.Info("I'm hungry!");
            yield return SwitchToFood();

            if (GameController.Player.GetHeldItem is ItemFood)
            {
                Log.Info($"Eating {GameController.Player.GetHeldItem.Name}");
                ActionManager.UseItem();
                for (var i = 0; i < 32; i++)
                {
                    yield return Awaiter;
                    ActionManager.UpdatePosition();
                }
            }
            yield return null;
            yield return WaitForSeconds(300);
        }

        public void GenerateStrings()
        {
            var name = GameController.Player.Name;
            TeleportCheckpoint = $"/warp blpoint{name}";
            TeleportHome = $"/warp {Settings.HomeWarp}";
            TeleportBack = $"/warp {Settings.BackWarp}";
            CreateCheckpoint = $"/warp pcreate blpoint{name}";
            RemoveCheckpoint = $"/warp remove blpoint{name}";
        }

        public override int GetPriority()
        {
            return 0;
        }

        public IEnumerator SaveCurrentLocation()
        {
            Log.Info("Perform homerun");
            ActionManager.SendChatMessage(RemoveCheckpoint);
            yield return WaitForSeconds(4000);

            ActionManager.SendChatMessage(CreateCheckpoint);
            yield return WaitForSeconds(4000);

            ActionManager.SendChatMessage(TeleportHome);
            yield return WaitForSeconds(1000);
        }

        public void Stop()
        {
            Routine = null;
        }

        public IEnumerator StoreItems()
        {
            ActionManager.RightClick(Chest);
            yield return WaitForSeconds(1500);

            foreach (var slot in GameController.Player.CurrentContainer.IndexedInventory)
            {
                
                if (slot.Item == null || Settings.KeepItems.Contains(slot.Item.Id) || slot.Item.Id == -1) continue;

                var freeSlot = GameController.Player.CurrentContainer.ContainerFreeSlot;

                if (freeSlot == -1) break;
                yield return ActionManager.ExchangeInventorySlots(slot.Slot, freeSlot);
                ActionManager.UpdatePosition();
                yield return Awaiter;
                yield return Awaiter;
            }

            ActionManager.CloseWindow();

            yield return WaitForSeconds(500);
        }

        private IEnumerator HomeRun()
        {
            yield return SaveCurrentLocation();
            yield return StoreItems();

            ActionManager.SendChatMessage(TeleportCheckpoint);

            yield return WaitForSeconds(2500);
        }

        private bool InventoryIsFool(object context)
        {
            if (GameController.Player.CurrentContainer.InventoryFreeSlot != -1 && !Store) return false;

            Store = false;
            Routine = StartRoutine(HomeRun());
            return true;
        }

        private bool IsDead(object context)
        {
            if (GameController.Player.Health > 0) return false;
            Routine = StartRoutine(Ressurect());
            return true;
        }

        private bool IsHungry(object context)
        {
            if (GameController.Player.Food > MinFoodDanger) return false;

            Routine = StartRoutine(Feed());
            return true;
        }

        private bool IsInDanger(object context)
        {
            if (GameController.Player.Health > MinHealthDanger) return false;
            Routine = StartRoutine(SaveFromDanger());
            return true;
        }

        private IEnumerator Ressurect()
        {
            var sb = new StringBuilder();
            sb.AppendLine("I'm dead!");
            sb.AppendLine("----Nearby entities: ");
            foreach (var entity in GameController.GetEntities<LivingEntity>())
            {
                sb.AppendLine(entity.ToString());
            }

            Log.Info(sb.ToString);

            Console.WriteLine("Stopping routines!");

            GameController.AiHandler.Mover.Stop();
            GameController.AiHandler.Digger.EnableDig = false;

            ActionManager.Respawn();
            ActionManager.UpdatePosition();
            yield return WaitForSeconds(1000);
        }

        private RunStatus RunRoutine(object context)
        {
            lock (monitor)
            {
                if (Routine == null || !Routine.MoveNext())
                    return RunStatus.Success;
                return RunStatus.Running;
            }
        }

        private IEnumerator SaveFromDanger()
        {
            Log.Info("We are in danger, runaway home!");
            ActionManager.SendChatMessage(TeleportHome);
            yield return WaitForSeconds(1000);
            yield return WaitForHealing();
            Log.Info("We are safe now, return back to work");
            ActionManager.SendChatMessage(TeleportBack);
            yield return WaitForSeconds(2500);
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

                yield return Awaiter;
                yield break;
            }

            var items =
                GameController.Player.Inventory.IndexedInventory;
            item = items.FirstOrDefault(slot => slot.Item is ItemFood);

            if (item == null)
            {
                MinFoodDanger = -1;
                Log.Info("No food in inventory! Disabling autofeeding");
                yield break;
            }

            Console.WriteLine($"Selecting {item.Item.Name} at {item.Slot}");
            yield return ActionManager.ExchangeInventorySlots(GameController.Player.HeldItem, item.Slot);
            ActionManager.UpdatePosition();

            yield return Awaiter;
        }

        private IEnumerator WaitForHealing()
        {
            var player = GameController.Player;
            while (player.Health < 20)
            {
                if (player.Food < 18 && MinFoodDanger > 0)
                    yield return Feed();

                ActionManager.UpdatePosition();
                yield return null;
            }
        }
    }
}