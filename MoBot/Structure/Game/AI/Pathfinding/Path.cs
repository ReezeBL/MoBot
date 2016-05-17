using System.Collections.Generic;

namespace MoBot.Structure.Game.AI.Pathfinding
{
    class Path
    {
        readonly Queue<PathPoint> _pathQueue = new Queue<PathPoint>();
        public void AddPoint(PathPoint point)
        {
            _pathQueue.Enqueue(point);
        }

        public PathPoint Dequeue()
        {
            return HasNext() ? _pathQueue.Dequeue() : null;
        }

        public Path(List<PathPoint> list)
        {
            _pathQueue = new Queue<PathPoint>(list);
        }

        public bool HasNext()
        {
            return _pathQueue.Count > 0;
        }
        public Path()
        {
        }
    }
}
