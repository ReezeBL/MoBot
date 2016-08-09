using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using MoBot.Structure.Game.AI.Pathfinding;
using MoBot.Structure.Game.Items;
using NLog;
using TreeSharp;
using Action = TreeSharp.Action;

namespace MoBot.Structure.Game.AI.Tasks
{
    public class Mover : Task
    {
        private IEnumerator _mover;
        private Logger _logger = Program.GetLogger();

        private bool DoNext(object context)
        {
            return _mover != null && _mover.MoveNext();
        }

        public Mover()
        {
            _root = new Action(RoutineTick);
        }

        public void SetDestination(PathPoint endPoint)
        {
            _mover = ActionManager.MoveRoutineS(endPoint);
        }

        public void SetShovelDestination(PathPoint endPoint)
        {
            _mover = Routine(endPoint);
        }

        private RunStatus RoutineTick(object context)
        {
            if(_mover == null || !_mover.MoveNext())
                return RunStatus.Failure;
           
            return _mover.Current != null ? RunStatus.Running : RunStatus.Success;
        }

        private IEnumerator Routine(PathPoint destination)
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
                        foreach (var p in DigTo(point.X, point.Y + 1, point.Z)) yield return p;
                        continue;
                    }

                    if (GameController.World.IsBlockFree(point.X, point.Y, point.Z)) continue;
                    foreach (var p in DigTo(point.X, point.Y, point.Z)) yield return p;
                }

                Console.WriteLine($"Moving to {point}");
                ActionManager.SetPlayerPos(point);
                for(int i=0;i<2;i++)
                    yield return null;
            }
        }

        private IEnumerable DigTo(int x, int y, int z)
        {
            GameBlock block = GameController.World.GetBlock(x, y, z);

            Console.WriteLine($"Digging block {block.Name} : {{{x} | {y} | {z} }}");

            foreach (var p in SwitchTool(block)) yield return p;

            ActionManager.StartDigging(x, y, z);
            foreach (var tick in DigBlock(block))
                yield return tick;
            ActionManager.FinishDigging(x, y, z);

            for (int i = 0; i < 6; i++)
                yield return null;
        }

        private IEnumerable SwitchTool(GameBlock block)
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

            foreach (var p in ActionManager.ExchangeInventorySlots(item.Slot, GameController.Player.HeldItem))
            {
                yield return _awaiter;
            }

            yield return _awaiter;
        }

        private IEnumerable DigBlock(GameBlock block)
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
