using System;
using System.Collections;
using System.Diagnostics;
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

                Console.WriteLine("Moving");
                ActionManager.SetPlayerPos(point);
                for(int i=0;i<2;i++)
                    yield return null;
            }
        }

        private IEnumerable DigTo(int x, int y, int z)
        {
            GameBlock block = GameController.World.GetBlock(x, y, z);
            Console.WriteLine($"Digging down block {block.Id}:{block.Name}");

            foreach (var p in SwitchTool(block)) yield return p;

            ActionManager.StartDigging(x, y, z);

            float power = GameController.Player.GetHeldItem.GetItemStrength(block);
            long waitTime = Item.GetWaitTime(power);

            Console.WriteLine($"Idle for {waitTime} ms");
            var seconds = WaitForSeconds(waitTime + 50);

            while (seconds.MoveNext())
                yield return seconds.Current;

            ActionManager.FinishDigging(x, y, z);
        }

        private IEnumerable SwitchTool(GameBlock block)
        {
            var heldItem = GameController.Player.GetHeldItem;
            var tool = heldItem as ItemTool;

            if (tool != null && tool.IsItemEffective(block))
            {
                yield break;
            }
            int index = 0;
            var item = GameController.Player.Inventory.Select(stack => new {Item = stack.Item, SlotNumber = index++}).Where(slot =>
            {
                var lambdaTool = slot.Item as ItemTool;
                return lambdaTool != null && lambdaTool.IsItemEffective(block);
            }).FirstOrDefault();
            if (item == null) yield break;

            foreach (var p in ActionManager.ExchangeInventorySlots(item.SlotNumber, GameController.Player.HeldItem))
                yield return p;
        }
    }
}
