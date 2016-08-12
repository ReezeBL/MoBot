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
        private IEnumerator _mover;

        public Mover()
        {
            _root = new Action(RoutineTick);
        }

        public void SetDestination(Location endPoint)
        {
            _mover = StartRoutine(MoveRoutine(endPoint));
        }

        public void Stop()
        {
            _mover = null;
        }

        public void SetShovelDestination(Location endPoint, Path preBuild = null)
        {
            _mover = StartRoutine(ShovelRoutine(endPoint, preBuild));
        }

        private RunStatus RoutineTick(object context)
        {
            if(_mover == null || !_mover.MoveNext())
                return RunStatus.Failure;
           
            return _mover.Current != null ? RunStatus.Running : RunStatus.Success;
        }

        private IEnumerator MoveRoutine(Location destination)
        {
            Path path = PathFinder.Shovel(GameController.Player.Position, destination, false, false);
            if (path == null)
            {
                Console.WriteLine("Can't resolve path!");
                yield break;
            }
            foreach (var point in path)
            {
                Console.WriteLine($"Moving to {point}");
                ActionManager.SetPlayerPos(point);
                for (int i = 0; i < 2; i++)
                    yield return null;
            }
        }

        private IEnumerator ShovelRoutine(Location destination, Path preBuild)
        {
            Path path = preBuild ?? PathFinder.Shovel(GameController.Player.Position, destination);
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
                }

                Console.WriteLine($"Moving to {point}");
                ActionManager.SetPlayerPos(point);
                for(int i=0;i<2;i++)
                    yield return null;
            }
        }

        private IEnumerator DigTo(int x, int y, int z)
        {
            Block block = Block.GetBlock(GameController.World.GetBlock(x, y, z));
            Console.WriteLine($"Digging block {block.Name} : {{{x} | {y} | {z} }}");

            yield return SwitchTool(block);

            ActionManager.StartDigging(x, y, z);
            yield return DigBlock(block);
            ActionManager.FinishDigging(x, y, z);

            ActionManager.UpdatePosition();
            yield return WaitForSeconds(150);
            ActionManager.UpdatePosition();
            yield return WaitForSeconds(150);

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
                yield return _awaiter;
                yield break;
            }

            var items =
               GameController.Player.Inventory.IndexedInventory;
            item = items.Where(IsEffectiveTool(block)).FirstOrDefault();
            if (item == null) yield break;

            Console.WriteLine($"Selecting {item.Item.Name} at {item.Slot}");

            yield return ActionManager.ExchangeInventorySlots(item.Slot, GameController.Player.HeldItem);
            ActionManager.UpdatePosition();

            yield return _awaiter;
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
            float blockHealth = 1f;
            while (blockHealth > 0)
            {
                blockHealth -= GameController.Player.GetDigSpeed(block);
                yield return _awaiter;
            }
        }
    }
}
