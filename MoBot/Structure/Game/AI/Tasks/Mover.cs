using System.Collections;
using MoBot.Structure.Game.AI.Pathfinding;
using TreeSharp;

namespace MoBot.Structure.Game.AI.Tasks
{
    class Mover : Task
    {
        private IEnumerator _mover;

        private bool DoNext(object context)
        {
            return _mover != null && _mover.MoveNext();
        }

        public Mover()
        {
            _root = new Decorator(DoNext, new Action(o => RunStatus.Success));
        }

        public void SetDestination(PathPoint endPoint)
        {
            _mover = ActionManager.MoveRoutineS(endPoint);
        }

    }
}
