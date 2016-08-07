using System;
using System.Collections;
using System.Diagnostics;
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
            foreach (var point in path)
            {
                while (!GameController.World.CanMoveTo(point.X, point.Y, point.Z))
                {
                    if (!GameController.World.IsBlockFree(point.X, point.Y + 1, point.Z))
                    {
                        GameBlock block = GameController.World.GetBlock(point.X, point.Y + 1, point.Z);
                        Console.WriteLine($"Digging down block {block.Id}:{block.Name}");
                        ActionManager.StartDigging(point.X, point.Y + 1, point.Z);

                        float power = GameController.Player.GetHeldItem.GetItemStrength(block);
                        long waitTime = Item.GetWaitTime(power);

                        Console.WriteLine($"Idle for {waitTime} ms");
                        var s = WaitForSeconds(waitTime + 50);
                        while (s.MoveNext())
                            yield return s.Current;

                        ActionManager.FinishDigging(point.X, point.Y + 1, point.Z);
                        continue;
                    }

                    if (!GameController.World.IsBlockFree(point.X, point.Y, point.Z))
                    {
                        GameBlock block = GameController.World.GetBlock(point.X, point.Y, point.Z);
                        Console.WriteLine($"Digging down block {block.Id}:{block.Name}");
                        ActionManager.StartDigging(point.X, point.Y, point.Z);

                        float power = GameController.Player.GetHeldItem.GetItemStrength(block);
                        long waitTime = Item.GetWaitTime(power);

                        Console.WriteLine($"Idle for {waitTime} ms");
                        var s = WaitForSeconds(waitTime + 50);
                        while (s.MoveNext())
                            yield return s.Current;

                        ActionManager.FinishDigging(point.X, point.Y, point.Z);
                        continue;
                    }
                }

                Console.WriteLine("Moving");
                ActionManager.SetPlayerPos(point);
                for(int i=0;i<2;i++)
                    yield return null;
            }
        }
    }
}
