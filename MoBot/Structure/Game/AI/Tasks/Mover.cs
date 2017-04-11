using System;
using System.Collections;
using System.Linq;
using MoBot.Structure.Game.AI.Pathfinding;
using MoBot.Structure.Game.Items;
using TreeSharp;
using Action = TreeSharp.Action;

namespace MoBot.Structure.Game.AI.Tasks
{
    public class Mover : Task
    {
        private IEnumerator mover;
        private readonly object monitor = new object();

        private IEnumerator _Mover
        {
            get
            {
                lock(monitor)
                    return mover;
            }

            set
            {
                lock (monitor)
                    mover = value;
            }
        }

        public Mover()
        {
            Root = new Action(RoutineTick);
        }

        public void SetDestination(Location endPoint)
        {
            _Mover = StartRoutine(MoveRoutine(endPoint));
        }

        public void Stop()
        {
            _Mover = null;
        }

        public void SetShovelDestination(Location endPoint, Path preBuild = null)
        {
            _Mover = StartRoutine(ShovelRoutine(endPoint, preBuild));
        }

        private RunStatus RoutineTick(object context)
        {
            lock (monitor)
            {
                if (_Mover == null || !_Mover.MoveNext())
                    return RunStatus.Failure;

                return _Mover.Current != null ? RunStatus.Running : RunStatus.Success;
            }
        }

        private IEnumerator MoveRoutine(Location destination)
        {
            var path = PathFinder.Shovel(GameController.Player.Position, destination, false, false);
            if (path == null)
            {
                Console.WriteLine("Can't resolve path!");
                yield break;
            }
            foreach (var point in path)
            {
                Console.WriteLine($"Moving to {point}");
                ActionManager.SetPlayerPos(point);
                yield return null;
                yield return null;
            }
        }

        private IEnumerator ShovelRoutine(Location destination, Path preBuild)
        {
            var path = preBuild ?? PathFinder.Shovel(GameController.Player.Position, destination);
            if(path == null)
                yield break;
            foreach (var point in path)
            {
                while (!GameController.World.CanMoveTo(point.X, point.Y, point.Z))
                {
                    if (!GameController.World.IsBlockFree(point.X, point.Y + 1, point.Z))
                    {
                        yield return DigTo(point.X, point.Y + 1, point.Z);
                        continue;
                    }

                    if (GameController.World.IsBlockFree(point.X, point.Y, point.Z)) continue;
                    yield return DigTo(point.X, point.Y, point.Z);

                    yield return null;
                }

                Console.WriteLine($"Moving to {point}");
                ActionManager.SetPlayerPos(point);
                yield return null;
                yield return null;
            }
        }

        private IEnumerator DigTo(int x, int y, int z)
        {
            var block = Block.GetBlock(GameController.World.GetBlock(x, y, z));
            Console.WriteLine($"Digging block {block.Name} : {{{x} | {y} | {z} }}");

            yield return SwitchTool(block);

            ActionManager.StartDigging(x, y, z);
            yield return Awaiter;
            ActionManager.UpdatePosition();
            ActionManager.FinishDigging(x, y, z);
            yield return Awaiter;
            ActionManager.UpdatePosition();
            yield return DigBlock(block);

            yield return WaitForSeconds(350);
        }

        private IEnumerator SwitchTool(Block block)
        {
            var heldItem = GameController.Player.GetHeldItem;
            var tool = heldItem as ItemTool;

            if (tool != null && tool.CanHarvest(block))
            {
                yield break;
            }
            var belt = GameController.Player.Inventory.Belt;
            var item = belt.Where(IsEffectiveTool(block)).FirstOrDefault();

            if (item != null)
            {
                Console.WriteLine($"Selecting {item.Item.Name} in the belt {item.Slot}");
                ActionManager.SelectBeltSlot(item.Slot);
                ActionManager.UpdatePosition();
                yield return Awaiter;
                yield break;
            }

            var items =
               GameController.Player.Inventory.IndexedInventory;
            item = items.Where(IsEffectiveTool(block)).FirstOrDefault();
            if (item == null) yield break;

            Console.WriteLine($"Selecting {item.Item.Name} at {item.Slot}");

            yield return ActionManager.ExchangeInventorySlots(item.Slot, GameController.Player.HeldItem);
            ActionManager.UpdatePosition();

            yield return Awaiter;
        }

        private static Func<Container.IndexedItem, bool> IsEffectiveTool(Block block)
        {
            return slot =>
            {
                var lambdaTool = slot.Item as ItemTool;
                return lambdaTool != null && lambdaTool.CanHarvest(block);
            };
        }

        private IEnumerator DigBlock(Block block)
        {
            var blockHealth = 1f;
            while (blockHealth > 0)
            {
                blockHealth -= GameController.Player.GetDigSpeed(block);
                yield return Awaiter;
            }
        }
    }
}
