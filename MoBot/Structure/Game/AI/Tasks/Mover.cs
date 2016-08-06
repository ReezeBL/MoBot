using System.Collections;
using System.Diagnostics;
using MoBot.Structure.Game.AI.Pathfinding;
using TreeSharp;

namespace MoBot.Structure.Game.AI.Tasks
{
    public class Mover : Task
    {
        private IEnumerator _mover;
        private readonly object _awaiter = new object();

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
                        ActionManager.StartDigging(point.X, point.Y + 1, point.Z);

                        float power = GameController.Player.GetHeldItem.GetItemEffectivness(block);
                        yield return WaitForSeconds(Item.GetWaitTime(power));

                        ActionManager.FinishDigging(point.X, point.Y + 1, point.Z);
                        continue;
                    }

                    if (!GameController.World.IsBlockFree(point.X, point.Y, point.Z))
                    {
                        GameBlock block = GameController.World.GetBlock(point.X, point.Y, point.Z);
                        ActionManager.StartDigging(point.X, point.Y, point.Z);

                        float power = GameController.Player.GetHeldItem.GetItemEffectivness(block);
                        yield return WaitForSeconds(Item.GetWaitTime(power));

                        ActionManager.FinishDigging(point.X, point.Y, point.Z);
                        continue;
                    }

                    ActionManager.SetPlayerPos(point);
                    yield return null;
                }
                
            }
        }

        private IEnumerator WaitForSeconds(long milliseconds)
        {
            var time = new Stopwatch();
            time.Start();
            while (time.ElapsedMilliseconds < milliseconds)
                yield return _awaiter;
        }
    }
}
