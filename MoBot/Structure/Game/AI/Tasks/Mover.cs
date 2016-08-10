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
            _mover = StartRoutine(ActionManager.MoveRoutineS(endPoint));
        }

        public void SetShovelDestination(Location endPoint)
        {
            _mover = StartRoutine(Routine(endPoint));
        }

        private RunStatus RoutineTick(object context)
        {
            if(_mover == null || !_mover.MoveNext())
                return RunStatus.Failure;
           
            return _mover.Current != null ? RunStatus.Running : RunStatus.Success;
        }

        private IEnumerator Routine(Location destination)
        {
            Path path = PathFinder.Shovel(GameController.Player.Position, destination);
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
            GameBlock block = GameBlock.GetBlock(GameController.World.GetBlock(x, y, z));
            Console.WriteLine($"Digging block {block.Name} : {{{x} | {y} | {z} }}");

            yield return SwitchTool(block);

            ActionManager.StartDigging(x, y, z);
            yield return DigBlock(block);

            ActionManager.FinishDigging(x, y, z);

            for (int i = 0; i < 10; i++)
                yield return null;
        }

        private IEnumerator SwitchTool(GameBlock block)
        {
            var heldItem = GameController.Player.GetHeldItem;
            var tool = heldItem as ItemTool;

            if (tool != null && tool.CanHarvest(block))
            {
                yield break;
            }
            var items = 
               GameController.Player.Inventory.IndexedInventory;
            var item = items.Where(slot =>
            {
                var lambdaTool = slot.Item as ItemTool;
                return lambdaTool != null && lambdaTool.CanHarvest(block);
            }).FirstOrDefault();
            if (item == null) yield break;

            Console.WriteLine($"Selecting {item.Item.Name} at {item.Slot}");

            yield return ActionManager.ExchangeInventorySlots(item.Slot, GameController.Player.HeldItem);

            yield return _awaiter;
        }

        private IEnumerator DigBlock(GameBlock block)
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
